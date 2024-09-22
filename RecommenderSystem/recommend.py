# Thư viện
import pandas as pd
import numpy as np

from sklearn.metrics.pairwise import linear_kernel, cosine_similarity
from sklearn.feature_extraction.text import TfidfVectorizer

from scipy import sparse

class HybridRecommender:
    def __init__(self, products, ratings, k = 10):
        self.cb_recommender = self.CB(products)
        self.cb_recommender.fit()

        self.cf_recommender = self.CF(evaluates = ratings, k = k)
        self.cf_recommender.clear_data()
        self.cf_recommender.fit()

        self.k = k

        self.products = products

    class CB(object):
        def __init__(self, Y_data, k = 10):
            self.Y_data = Y_data

        def tfidf_matrix(self):
            """
                Dùng hàm "TfidfVectorizer" để chuẩn hóa dữ liệu với:
                    + analyzer='word': chọn đơn vị trích xuất là word
                    + ngram_range=(1, 1): mỗi lần trích xuất 1 word
                    + min_df=0: tỉ lệ word không đọc được là 0
               
                Kết hợp các ma trận TF-IDF với các trọng số khác nhau
            """
            tf = TfidfVectorizer(analyzer='word', ngram_range=(1, 1), min_df=0.0)

            prodcut_name_list = [product['TenSanPham'] for product in self.Y_data]
            product_type_list = [product['LoaiSanPham'] for product in self.Y_data]
            product_recipe_list = [product['CongThuc'] for product in self.Y_data]

            tfidf_matrix_product_name= tf.fit_transform(prodcut_name_list)
            tfidf_matrix_product_type = tf.fit_transform(product_type_list)
            tfidf_matrix_product_recipe = tf.fit_transform(product_recipe_list)

            self.combined_tfidf_matrix = sparse.hstack((tfidf_matrix_product_recipe * 0.2, tfidf_matrix_product_name * 0.5, tfidf_matrix_product_type * 0.3))

        def cosine_sim(self):
            """
                Dùng hàm "linear_kernel" để tạo thành ma trận hình vuông với số hàng và số cột là số lượng món ăn
                để tính toán điểm tương đồng giữa từng bộ món ăn với nguyên liệu tương ứng
            """
            self.new_cosine_sim = linear_kernel(self.combined_tfidf_matrix, self.combined_tfidf_matrix)

        def refresh(self):
            self.tfidf_matrix()
            self.cosine_sim()

        def fit(self):
            self.refresh()

        def recommend(self, id):
            idx = next((i for i, Y in enumerate(self.Y_data) if Y['MaSanPham'] == id), None)
            sim_scores = list(enumerate(self.new_cosine_sim[idx]))
            sim_scores = sorted(sim_scores, key=lambda x: x[1], reverse=True)
            product_indices = [i[0] for i in sim_scores]

            products = [self.Y_data[index] for index in product_indices]

            return products

    class CF(object):
        """
        Một là xác định mức độ quan tâm của mỗi user tới một item dựa trên mức độ quan tâm của users gần giống nhau (similar users) tới item đó
        còn được gọi là User-user collaborative filtering.

        Hai là thay vì xác định user similarities, hệ thống sẽ xác định item similarities.
        Từ đó, hệ thống gợi ý những items gần giống với những items
        mà user có mức độ quan tâm cao.
        """
        def __init__(self, evaluates, k = 10, dist_func = cosine_similarity, uuCF = 1):
            self.uuCF = uuCF # user-user (1) or item-item (0) CF
            self.k = k
            self.dist_func = dist_func
            self.evaluates = evaluates
            self.Ybar_data = None

        def clear_data(self):
            for evaluate in self.evaluates:
                evaluate['MaKhachHang'] = int(evaluate['MaKhachHang'][2:])
                evaluate['MaSanPham'] = int(evaluate['MaSanPham'][2:])
                evaluate['DiemDanhGia'] = int(evaluate['DiemDanhGia'])

            self.evaluates = np.array([
                [item['MaKhachHang'], item['MaSanPham'], item['DiemDanhGia']]
                for item in self.evaluates
            ])

            evaluates_data = np.array(self.evaluates)
            
            self.cusomter = np.unique(evaluates_data[:, 0])
            self.cusomter.sort()
            indices = np.searchsorted(self.cusomter, evaluates_data[:, 0])
            evaluates_data[:, 0] = indices

            self.product = np.unique(evaluates_data[:, 1])
            self.product.sort()
            indices = np.searchsorted(self.product, evaluates_data[:, 1])
            evaluates_data[:, 1] = indices

            self.Y_data = evaluates_data if self.uuCF else evaluates_data[:, [1, 0, 2]]
            self.users = np.unique(self.Y_data[:, 0])
            self.items = np.unique(self.Y_data[:, 1])
            self.n_users = len(self.users)
            self.n_items = len(self.items)

        def normalize_Y(self):
            """
            Tính similarity giữa các items bằng cách tính trung bình cộng ratings giữa các items.
            Sau đó thực hiện chuẩn hóa bằng cách trừ các ratings đã biết của item cho trung bình cộng
            ratings tương ứng của item đó, đồng thời thay các ratings chưa biết bằng 0.
            """
            self.Ybar_data = self.Y_data.copy()
            self.mu = {}

            for user in self.users:
                ids = np.where(self.Y_data[:, 0] == user)[0].astype(np.int32)
                ratings = self.Y_data[ids, 2]
                m = np.mean(ratings)
                if np.isnan(m):
                    m = 0 
                self.mu[user] = m
                self.Ybar_data[ids, 2] = ratings - self.mu[user]

            self.Ybar = sparse.coo_matrix((self.Ybar_data[:, 2],
                (self.Ybar_data[:, 1], self.Ybar_data[:, 0])), (self.n_items, self.n_users))

            self.Ybar = self.Ybar.tocsr()

        def similarity(self):
            """
            Tính độ tương đồng giữa các user và các item
            """
            self.S = self.dist_func(self.Ybar.T, self.Ybar.T)

        def refresh(self):
            """
            Normalize data and calculate similarity matrix again (after
            some few ratings added)
            """
            self.normalize_Y()
            self.similarity()

        def fit(self):
            self.refresh()

        def __pred(self, u, i, normalized = 1):
            """
            Dự đoán ra ratings của các users với mỗi items.
            """
            # Bước 1: tìm tất cả người dùng đã đánh giá i
            ids = np.where(self.Y_data[:, 1] == i)[0].astype(np.int32)
            # Bước 2: 
            users_rated_i = (self.Y_data[ids, 0]).astype(np.int32)
            # Bước 3: tìm sự tương đồng giữa người dùng hiện tại và những người khác
            # đã đánh giá i
            sim = self.S[u, users_rated_i]
            # Bước 4: tìm k người dùng giống nhau nhất
            a = np.argsort(sim)[-self.k:]
            # và mức độ tương đồng tương ứng
            nearest_s = sim[a]
            # Mỗi người dùng 'gần' đã đánh giá sản phẩm như thế nào?
            r = self.Ybar[i, users_rated_i[a]]
            if normalized:
                # thêm một số nhỏ, ví dụ 1e-8, để tránh chia cho 0
                return (r*nearest_s)[0]/(np.abs(nearest_s).sum() + 1e-8)

            return (r*nearest_s)[0]/(np.abs(nearest_s).sum() + 1e-8) + self.mu[u]

        def pred(self, u, i, normalized = 1):
            """
            dự đoán xếp hạng của người dùng u cho mục i (chuẩn hóa)
            nếu bạn cần
            """
            if self.uuCF: return self.__pred(u, i, normalized)
            return self.__pred(i, u, normalized)

        def recommend(self, u):
            """
            Xác định tất cả các mục nên được đề xuất cho người dùng u.
             Quyết định được đưa ra dựa trên tất cả những gì:
             self.pred(u, i) > 0. Giả sử chúng ta đang xem xét các mục
             bạn chưa đánh giá được.
            """
            ids = np.where(self.Y_data[:, 0] == u)[0]
            items_rated_by_u = self.Y_data[ids, 1].tolist()
            recommended_items = []
            for i in range(self.n_items):
                if i not in items_rated_by_u:
                    rating = self.__pred(u, i)
                    if rating > 0:
                        recommended_items.append(i)

            return recommended_items

        def update_data_recommend(self, recommended_items):
            """
            INPUT: Các chỉ số của phần tử
            OUTPUT: Các mã sản phẩm tương ứng
            """

            selected_values = [self.product[index] for index in recommended_items]
            recommended_values = ["SP{:04d}".format(value) for value in selected_values]

            return recommended_values

        def recommend_u(self, u):
            """
            INPUT: Mã ban đầu của khách hàng
            """
            # Tách số ra chuỗi
            number_u = int(u[2:])
            # Mã khách hàng sau khi nén ra
            indices = np.where(self.cusomter == number_u)[0]

            if (len(indices) > 0):
                return self.update_data_recommend(self.recommend(indices[0]))
            else:
                return []

    def recommend(self, user_id, product_id):
        recommendations = []

        if user_id:
            # Sử dụng Collaborative Filtering đầu tiên
            # Chỉ có các mã sản phẩm
            cf_recommendations = self.cf_recommender.recommend_u(user_id)
            cf_recommendations = [product for product in self.products if product['MaSanPham'] in cf_recommendations]
            recommendations = cf_recommendations

        if product_id:
            if len(recommendations) < self.k:
                # Nếu không đủ, sử dụng Content-based Filtering
                num_more = self.k - len(recommendations)
                cb_recommendations = self.cb_recommender.recommend(product_id)

                for product in cb_recommendations:
                    if product['MaSanPham'] == product_id:
                        continue

                    if num_more == 0:
                        break

                    if product not in recommendations:
                        recommendations.append(product)
                        num_more -= 1

        return recommendations
