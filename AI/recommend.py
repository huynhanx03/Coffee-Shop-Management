import numpy as np
from scipy.sparse import coo_matrix
from implicit.als import AlternatingLeastSquares

from sklearn.metrics.pairwise import cosine_similarity

from model_bert import bert_embedding

class CB(object):
    def __init__(self, Y_data, k=10):
        self.Y_data = Y_data
        self.k = k

    def embeddings_matrix(self):
        # Sử dụng BERTEmbedding để tạo vector cho các sản phẩm
        product_name_embeddings = np.vstack([bert_embedding.encode(product['TenSanPham']) for product in self.Y_data])
        product_type_embeddings = np.vstack([bert_embedding.encode(product['LoaiSanPham']) for product in self.Y_data])
        product_recipe_embeddings = np.vstack([bert_embedding.encode(product['CongThuc']) for product in self.Y_data])

        # Kết hợp các embedding với trọng số
        self.combined_embeddings = 0.7 * product_name_embeddings + 0.2 * product_type_embeddings + 0.1 * product_recipe_embeddings

    def calculate_similarity(self):
        # Tính toán ma trận cosine similarity giữa các sản phẩm
        self.similarity_matrix = cosine_similarity(self.combined_embeddings)

    def refresh(self):
        self.embeddings_matrix()
        self.calculate_similarity()

    def fit(self):
        self.refresh()

    def recommend(self, product_id):
        idx = next(i for i, Y in enumerate(self.Y_data) if Y['MaSanPham'] == product_id)

        return [self.Y_data[i] for i, _ in sorted(enumerate(self.similarity_matrix[idx]), key=lambda x: -x[1])[:self.k]]

class CF(object):
    """
    Collaborative Filtering sử dụng thuật toán ALS để phân rã ma trận rating thành ma trận yếu tố người dùng và sản phẩm.
    """
    def __init__(self, evaluates, factors=20, regularization=0.1, iterations=20, alpha=0.5, k=10):
        """
        factors: Số lượng yếu tố tiềm ẩn (latent factors) trong ALS
        regularization: Hệ số điều chuẩn để tránh overfitting trong ALS
        iterations: Số vòng lặp tối đa để ALS hội tụ
        alpha: Tỉ lệ alpha điều chỉnh trọng số của implicit feedback
        """
        self.evaluates = evaluates
        self.factors = factors
        self.regularization = regularization
        self.iterations = iterations
        self.alpha = alpha  # Độ quan trọng của implicit feedback
        self.user_factors = None
        self.item_factors = None
        self.k = k

    def clear_data(self):
        """
        Chuẩn hóa dữ liệu người dùng và sản phẩm
        """
        for evaluate in self.evaluates:
            evaluate['MaKhachHang'] = int(evaluate['MaKhachHang'][2:])
            evaluate['MaSanPham'] = int(evaluate['MaSanPham'][2:])
            evaluate['DiemDanhGia'] = int(evaluate['DiemDanhGia'])

        self.evaluates = np.array([
            [item['MaKhachHang'], item['MaSanPham'], item['DiemDanhGia']]
            for item in self.evaluates
        ])

        evaluates_data = np.array(self.evaluates)
        self.customers = np.unique(evaluates_data[:, 0])
        self.products = np.unique(evaluates_data[:, 1])

        # Ánh xạ người dùng và sản phẩm vào chỉ mục tuần tự
        customer_indices = np.searchsorted(self.customers, evaluates_data[:, 0])
        product_indices = np.searchsorted(self.products, evaluates_data[:, 1])

        self.Y_data = np.column_stack((customer_indices, product_indices, evaluates_data[:, 2]))
        self.n_users = len(self.customers)
        self.n_items = len(self.products)

    def build_rating_matrix(self):
        """
        Xây dựng ma trận ratings dạng sparse (thưa thớt).
        """
        data = self.Y_data[:, 2]
        row = self.Y_data[:, 0]  # user IDs
        col = self.Y_data[:, 1]  # item IDs
        self.R = coo_matrix((data, (row, col)), shape=(self.n_users, self.n_items))

    def apply_ALS(self):
        """
        Áp dụng thuật toán ALS để phân rã ma trận rating.
        """
        # Tạo mô hình ALS
        model = AlternatingLeastSquares(factors=self.factors, regularization=self.regularization, iterations=self.iterations)

        # Chuẩn bị dữ liệu dạng implicit feedback (với alpha điều chỉnh implicit)
        confidence = (self.R * self.alpha).tocsr()

        # Áp dụng ALS
        model.fit(confidence)

        # Lưu lại ma trận yếu tố người dùng và sản phẩm
        self.user_factors = model.user_factors
        self.item_factors = model.item_factors

    def refresh(self):
        self.clear_data()
        self.build_rating_matrix()
        self.apply_ALS()

    def fit(self):
        self.refresh()
    
    def predict(self, user, item):
        """
        Dự đoán rating cho một user và item bằng cách nhân ma trận yếu tố.
        """
        return self.user_factors[user, :].dot(self.item_factors[item, :].T)

    def recommend(self, u):
        """
        Đề xuất top N sản phẩm cho người dùng u.
        """
        user_indices = np.where(self.customers == int(u[2:]))[0]

        user_index = user_indices[0]

        # Tính toán điểm dự đoán cho từng sản phẩm bằng hàm predict
        scores = np.array([self.predict(user_index, i) for i in range(self.n_items)])

        # Sắp xếp sản phẩm theo điểm dự đoán từ cao đến thấp
        top_items = np.argsort(scores)[::-1][:self.k]
        
        return self.update_data_recommend([self.products[i] for i in top_items])
    
    def update_data_recommend(self, recommended_items):
        """
        Chuyển đổi các chỉ số của sản phẩm thành mã sản phẩm ban đầu.
        """
        recommended_values = ["SP{:04d}".format(item) for item in recommended_items]
        return recommended_values

class HybridRecommender:
    def __init__(self, products, ratings, k = 10):
        self.cb_recommender = CB(products, k)
        self.cb_recommender.fit()

        self.cf_recommender = CF(evaluates = ratings, k = k)
        self.cf_recommender.fit()

        self.k = k

        self.products = products

    def recommend(self, user_id, product_id):
        recommendations = []

        if user_id:
            # Sử dụng Collaborative Filtering đầu tiên
            # Chỉ có các mã sản phẩm
            cf_recommendations = self.cf_recommender.recommend(user_id)
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
