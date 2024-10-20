import os
import pandas as pd

class CreateData:
    def __init__(self):
        self.begin2s = [
            '', 
            'cho mình hỏi', 
            'cho mình thắc mắc', 
            'cho tớ hỏi', 
            'mình hỏi chút', 
            'có thể cho mình biết', 
            'bạn cho mình hỏi', 
            'cậu có thể cho mình biết', 
            'mình muốn hỏi', 
            'mình có một câu hỏi', 
            'bạn có thể trả lời giúp mình', 
            'bạn cho mình xin thông tin về', 
            'mình thắc mắc chút', 
            'có thể hỏi bạn về', 
            'mình có điều muốn hỏi', 
            'bạn giúp mình với', 
            'bạn có thể cho tớ thông tin về', 
            'mình cần bạn giải đáp', 
            'có thể hỗ trợ mình với', 
            'có thể giúp mình giải đáp', 
            'mình muốn tìm hiểu về', 
            'cậu giúp mình với', 
            'bạn có thể cung cấp thông tin về', 
            'mình muốn biết', 
            'mình thắc mắc muốn hỏi bạn', 
            'mình cần chút thông tin về', 
            'mình muốn hỏi bạn chút thông tin về', 
            'có thể giải đáp giúp mình không', 
            'mình có một thắc mắc', 
            'bạn có thể trả lời thắc mắc của mình', 
            'mình đang thắc mắc về', 
            'bạn giúp mình chút được không'
        ]

        self.separas = [' ',
            # ', '
        ]

        self.ends = [
            '',
            # '.'
        ]

        self.objects = [
            'tôi', 'anh', 'em', 'mình', 'tớ', 'bạn'
        ]

        self.productNames = ['cà phê', 'espresso', 'americano']

        self.ingredients = ['sữa', 'đường', 'trứng', 'bột mì', 'bơ', 'mật ong', 'socola', 'phô mai', 'trà', 'cà phê']

    
    def createProductInfomationQuestions(self):
        # Câu hỏi dựa trên tiêu chí:
        # + Số lượng
        # + Thông tin ()
        # + Giá
        # + Mô tả
        # + Công thức

        productInformationQuestions = [
            # General Info
            'thông tin về "sản phẩm"'
            'thông tin chi tiết về "sản phẩm"',
            '"Sản phẩm" được mô tả như thế nào',
            'mô tả chi tiết về "sản phẩm" là gì',
            'có gì đặc biệt về "sản phẩm" mà mình nên biết',
            '"Sản phẩm" có tính năng nổi bật nào',
            'rõ hơn về "sản phẩm"',
            '"Sản phẩm" được giới thiệu ra sao',

            # Availability
            '"Sản phẩm" còn bao nhiêu ly',
            '"Sản phẩm" còn hàng không',
            'hiện tại có thể mua bao nhiêu "sản phẩm"',
            '"Sản phẩm" có sẵn không',
            '"sản phẩm" còn không',
            'có thể đặt hàng "sản phẩm" bây giờ được không',
            '"Sản phẩm" có đang trong kho hay không',
            'quán còn bán "sản phẩm" này không',
            'số lượng "sản phẩm" hiện tại trong kho là bao nhiêu',
            
            # Price
            '"Sản phẩm" giá bao nhiêu',
            'giá của "sản phẩm" là bao nhiêu',
            'bạn có thể cho mình biết giá của "sản phẩm" không',
            '"Sản phẩm" có giá hiện tại là bao nhiêu',
            'giá bán của "sản phẩm" hiện tại là bao nhiêu',
            'mình cần bao nhiêu tiền để mua "sản phẩm"',
            '"Sản phẩm" có mức giá như thế nào',
            'quán có thể báo giá "sản phẩm" giúp mình được không',
            'giá của "sản phẩm" đã giảm hay chưa',
            '"Sản phẩm" có khuyến mãi không và giá hiện tại là bao nhiêu',

            # Ingredients/Formula
            '"Sản phẩm" được làm từ nguyên liệu gì',
            'công thức chế biến "sản phẩm" là gì',
            '"Sản phẩm" được chế biến như thế nào',
            'nguyên liệu chính của "sản phẩm" là gì',
            'thành phần của "sản phẩm" không',
            'nguyên liệu nào được dùng để làm "sản phẩm"',
            '"Sản phẩm" có thành phần đặc trưng nào',
            '"Sản phẩm" được tạo thành từ những nguyên liệu gì',
            
            # Product Type
            '"Sản phẩm" thuộc loại nào',
            '"Sản phẩm" là sản phẩm loại gì',
            '"Sản phẩm" thuộc danh mục nào',
            '"Sản phẩm" là sản phẩm thuộc nhóm nào',
            '"Sản phẩm" có thuộc loại cao cấp không',
            '"Sản phẩm" có phân loại ra sao',
            '"Sản phẩm" là thuộc dòng sản phẩm gì',
            '"Sản phẩm" được xếp vào loại nào trên thị trường',
            '"Sản phẩm" là sản phẩm thuộc phân khúc nào'
        ]

        resultInformationQuestions = []

        for question in productInformationQuestions:
            for begin2 in self.begin2s:
                for separa in self.separas:
                    for end in self.ends:
                        for productName in self.productNames:
                            newQuestion = begin2 + ("" if begin2 == "" else separa) + question + end
                            
                            newQuestion = newQuestion.replace('"sản phẩm"', productName)
                            newQuestion = newQuestion.replace('"Sản phẩm"', productName)
                            
                            resultInformationQuestions.append(newQuestion)

        self.resultInformationQuestions = list(set(map(str.strip, filter(None, resultInformationQuestions))))

    def createProductTopSellQuestions(self):
        productTopSellQuestions = [
            'danh sách sản phẩm bán chạy hàng đầu',
            'sản phẩm được mua nhiều nhất là gì',
            'mhững sản phẩm nào đang hot nhất hiện nay',
            'danh sách sản phẩm bán chạy là gì',
            'sản phẩm nào bán chạy nhất',
            'sản phẩm nào được ưa chuộng nhất',
            'sản phẩm nào mọi người ưa chuộng nhất',
            'sản phẩm bán chạy nhất hiện tại là gì',
            'sản phẩm nào có lượng mua cao nhất',
            'quán đang bán chạy sản phẩm nào',
            'sản phẩm nào hot nhất hiện nay',
            'sản phẩm nào được khách hàng thích nhất',
            'có thể liệt kê sản phẩm bán chạy giúp mình không',
            'top sản phẩm bán chạy trong tháng này',
            'shop có thể cho biết sản phẩm bán chạy không',
            'sản phẩm nào được mua nhiều nhất gần đây',
            'những sản phẩm nào đang được khách hàng săn đón nhiều nhất',
            'sản phẩm nào đang dẫn đầu doanh số bán hàng',
            'sản phẩm nào được nhiều người mua nhất tuần này',
            'top sản phẩm hot nhất mà Quán đang bán là gì',
            'sản phẩm nào có lượt mua nhiều nhất trong thời gian gần đây',
            'sản phẩm nào đang được đánh giá cao nhất',
            'nạn có thể cho mình biết danh sách sản phẩm bán chạy nhất không',
            'sản phẩm nào có xu hướng bán chạy nhất trong tháng vừa rồi',
            'sản phẩm nào đang được mọi người ưa chuộng mua sắm hiện tại',
            'những sản phẩm nào đang nằm trong top doanh số',
            'mặt hàng nào có lượt mua nhiều nhất trong tháng này',
            'có sản phẩm nào được xem là siêu hot gần đây không',
            'sản phẩm nào đang là xu hướng mua sắm hiện nay',
            'mình có thể biết những sản phẩm nào bán chạy trong tuần vừa qua không',
            'những sản phẩm nào đang bán chạy nhất trong hệ thống của Quán',
            'quán có sản phẩm nào bán rất chạy mà mình nên biết không',
            'có sản phẩm nào mới xuất hiện nhưng đã bán rất chạy không',
            'top sản phẩm nào đang được khách hàng yêu thích nhất hiện nay',
            'có mặt hàng nào hiện tại đang dẫn đầu về lượng bán ra không',
            'những sản phẩm bán chạy nhất trong Quán là gì',
            'sản phẩm nào đang có nhiều phản hồi tích cực và bán chạy',
            'có sản phẩm nào đang hot và có lượng bán cao không',
            'quán có thể cho biết những sản phẩm nào đang bán chạy theo tuần không',
            'những sản phẩm nào đang bán chạy nhất trong danh mục hiện nay',
            'quán có thống kê về những sản phẩm bán chạy trong năm nay không',
            'sản phẩm nào đang được nhiều khách hàng quan tâm và mua sắm',
            'những sản phẩm nào có doanh thu cao nhất trong thời gian gần đây',
            'top sản phẩm bán chạy và được khách hàng yêu thích là gì',
            'những sản phẩm nào đang dẫn đầu doanh số bán hàng trong tháng này',
            'sản phẩm nào được mọi người mua nhiều nhất trong thời gian gần đây',
            'mặt hàng nào được xem là xu hướng và đang bán chạy nhất hiện tại'
        ]

        resultProductTopSellQuestions = []

        for question in productTopSellQuestions:
            for begin2 in self.begin2s:
                for separa in self.separas:
                    for end in self.ends:
                        newQuestion = begin2 + ("" if begin2 == "" else separa) + question + end

                        resultProductTopSellQuestions.append(newQuestion)

        self.resultProductTopSellQuestions = list(set(map(str.strip, filter(None, resultProductTopSellQuestions))))

    def createRecommendedProductQuestions(self):
        recommendedProductQuestions = [
            'gợi ý sản phẩm cho "object" với'
            'gợi ý cà phê tốt nhất',
            'bạn có thể gợi ý cà phê cho "object" không',
            'cà phê nào phù hợp với "object"',
            'cửa hàng gợi ý sản phẩm cho "object" được không',
            '"object" nên dùng cà phê nào',
            '"object" có thể thử cà phê nào',
            'gợi ý cho "object" cà phê tương tự với',
            '"object" thích cà phê nào thì nên mua gì',
            'cà phê tương tự "sản phẩm" là gì',
            'có cà phê nào giống "sản phẩm" không',
            'cho tôi biết cà phê nào phù hợp với "object"',
            'cà phê nào "object" nên xem xét',
            'gợi ý cà phê nào cho "object" tốt nhất',
            'bạn có biết cà phê nào cho "object" không?',
            '"object" muốn biết cà phê nào là tốt nhất',
            'có cà phê nào phù hợp với "object" không',
            '"object" muốn thử cà phê gì',
            'có cà phê nào mà "object" không nên bỏ lỡ không',
            'cà phê nào "object" nên thử',
            '"object" có thể tham khảo cà phê nào',
            'gợi ý cà phê mà "object" có thể thích',
        ]

        resultRecommendedProductQuestions = []

        for question in recommendedProductQuestions:
            for begin2 in self.begin2s:
                for separa in self.separas:
                    for end in self.ends:
                        for objectSelect in self.objects:
                            newQuestion = question + end

                            newQuestion = begin2 + ("" if begin2 == "" else separa) + newQuestion.replace('"object"', objectSelect)

                            if "sản phẩm" in newQuestion:
                                for productName in self.productNames:
                                    newQuestion = newQuestion.replace('"sản phẩm"', productName)
                                    resultRecommendedProductQuestions.append(newQuestion)
                            else:
                                resultRecommendedProductQuestions.append(newQuestion)

        self.resultRecommendedProductQuestions = list(set(map(str.strip, filter(None, resultRecommendedProductQuestions))))

    def createGreetingQuestions(self):
        begin0s = ["", "xin chào", "chào", "alo", "hey", "hello"]
        
        begin1s = ["", "bạn", "cậu", "em", "anh", "nhân viên", "quán", "shop", "mọi người"]

        resultGreetingQuestions = []

        for begin0 in begin0s:
            for begin1 in begin1s:
                for begin2 in self.begin2s:
                    for separa in self.separas:
                        for end in self.ends:
                            newQuestion = begin0 + ("" if begin0 == "" else separa) + begin1 + ("" if begin1 == "" else separa) + begin2 + ("" if begin2 == "" else separa) + end
                            resultGreetingQuestions.append(newQuestion)
        
        self.resultGreetingQuestions = list(set(map(str.strip, filter(None, resultGreetingQuestions))))

        return self.resultGreetingQuestions

    def createIngredientQuestion(self):
        ingredientQuestions = [
            'sản phẩm nào được chế biến bởi "ingredient"',
            'sản phẩm có chứa "ingredient" không',
            'có sản phẩm nào chứa "ingredient" không',
            'sản phẩm nào có "ingredient"',
            '"ingredient" có trong sản phẩm nào',
            'sản phẩm nào sử dụng "ingredient"',
            'sản phẩm nào được làm từ "ingredient"',
            'có sản phẩm nào được chế biến từ "ingredient"',
            'tôi có thể tìm sản phẩm chứa "ingredient" ở đâu',
            'sản phẩm chứa "ingredient" nào phổ biến nhất',
        ]

        resultIngredientQuestions = []

        for question in ingredientQuestions:
            for begin2 in self.begin2s:
                for separa in self.separas:
                    for end in self.ends:
                        for ingredient in self.ingredients:
                            newQuestion = begin2 + ("" if begin2 == "" else separa) + question + end
                            
                            newQuestion = newQuestion.replace('"ingredient"', ingredient)

                            resultIngredientQuestions.append(newQuestion)

        self.resultIngredientQuestions = list(set(map(str.strip, filter(None, resultIngredientQuestions))))

        return self.resultIngredientQuestions

    def createProductQualityQuestions(self):
        qualityQuestions = [
            '"product" có ngon không',
            '"product" có tốt không',
            'sản phẩm "product" có đáng mua không',
            '"product" có phải là sản phẩm ngon nhất không',
            'có nên thử "product" không',
            '"product" có phải là lựa chọn tốt không',
            '"product" có chất lượng không',
            '"product" có được nhiều người đánh giá cao không',
            'nên mua "product" hay không',
            'chất lượng của "product" thế nào',
            'bạn thấy "product" có đáng thử không',
            '"product" có được nhiều người yêu thích không',
            '"product" có đảm bảo không',
            '"product" có chất lượng ổn không',
            '"product" có đáng tin cậy không',
            '"product" có an toàn khi sử dụng không',
            'sản phẩm "product" có hợp lý không',
            'sản phẩm "product" có đáng để sử dụng không',
            'độ ngon của "product" như thế nào',
            'có nên lựa chọn "product" không',
            '"product" có đảm bảo chất lượng không',
            '"product" có thực sự tốt không',
            '"product" có giá trị sử dụng cao không',
            'nhiều người đánh giá "product" cao không',
            'sản phẩm "product" có xứng đáng không',
            'sản phẩm "product" có thực sự chất lượng không',
            '"product" có phù hợp với tôi không',
            '"product" có đáng để mua thử không',
            'có nên tin tưởng vào chất lượng của "product" không',
            'độ uy tín của "product" thế nào',
            'chất lượng của "product" có đảm bảo không',
            '"product" có đáng tiền không',
            'đánh giá về "product" có cao không',
            'người dùng nói gì về "product"',
            '"product" có tốt như lời đồn không',
            'sản phẩm "product" có phải là sự lựa chọn hoàn hảo không',
            '"product" có nổi tiếng không',
            '"product" có là sản phẩm phổ biến không',
            '"product1" với "product2" cái nào ngon hơn',
            '"product1" với "product2" khác nhau như thế nào',
            '"product1" có ngon hơn "product2" không',
            'nên chọn "product1" hay "product2"',
            'sản phẩm nào giữa "product1" và "product2" là tốt hơn',
            'giữa "product1" và "product2" thì cái nào phù hợp hơn',
            '"product1" và "product2" thì loại nào đáng mua hơn',
            '"product1" với "product2" thì cái nào phổ biến hơn',
            '"product1" với "product2" thì loại nào được ưa chuộng hơn',
            '"product1" và "product2" thì cái nào ngon hơn theo bạn'
        ]

        resultQualityQuestions = []

        for question in qualityQuestions:
            for begin2 in self.begin2s:
                for separa in self.separas:
                    for end in self.ends:
                        for product1 in self.productNames:
                            newQuestion = begin2 + ("" if begin2 == "" else separa) + question + end

                            if "product1" in newQuestion:
                                newQuestion = newQuestion.replace('"product1"', product1)

                                for product2 in self.productNames:
                                    if product1 != product2:
                                        newQuestion = newQuestion.replace('"product2"', product2)
                            else:
                                newQuestion = newQuestion.replace('"product"', product1)
                            
                            resultQualityQuestions.append(newQuestion)

        self.resultQualityQuestions = list(set(map(str.strip, filter(None, resultQualityQuestions))))

        return self.resultQualityQuestions


    def fit(self):
        self.createProductInfomationQuestions()
        self.createProductTopSellQuestions()
        self.createRecommendedProductQuestions()
        self.createIngredientQuestion()
        self.createProductQualityQuestions()
        self.createGreetingQuestions()

        self.printLenAllLabel()

    def printLenAllLabel(self):
        print("Số lượng dữ liệu của câu hỏi về thông tin sản phẩm:", len(self.resultInformationQuestions))
        print("Số lượng dữ liệu của câu hỏi về sản phẩm bán chạy:", len(self.resultProductTopSellQuestions))
        print("Số lượng dữ liệu của câu hỏi về gợi ý sản phẩm:", len(self.resultRecommendedProductQuestions))
        print("Số lượng dữ liệu của câu hỏi về nguyên liệu:", len(self.resultIngredientQuestions))
        print("Số lượng dữ liệu của câu hỏi về chất lượng:", len(self.resultQualityQuestions))
        print("Số lượng dữ liệu của câu hỏi về lời chào:", len(self.resultGreetingQuestions))

    def exportCsv(self):
        sizeInformationQuestion = len(self.resultInformationQuestions)
        sizeProductTopSellQuestion = len(self.resultProductTopSellQuestions)
        sizeRecommendedProductQuestion = len(self.resultRecommendedProductQuestions)
        sizeIngredientQuestion = len(self.resultIngredientQuestions)
        sizeQualityQuestion = len(self.resultQualityQuestions)
        sizeGreetingQuestion = len(self.resultGreetingQuestions)

        questions = self.resultInformationQuestions + self.resultProductTopSellQuestions + self.resultRecommendedProductQuestions + self.resultIngredientQuestions + self.resultQualityQuestions + self.resultGreetingQuestions

        print("Số lượng dữ liệu câu hỏi:", len(questions))

        infomations = [1] * sizeInformationQuestion + [0] * sizeProductTopSellQuestion + [0] * sizeRecommendedProductQuestion + [0] * sizeIngredientQuestion + [0] * sizeQualityQuestion + [0] * sizeGreetingQuestion
        productTopSells = [0] * sizeInformationQuestion + [1] * sizeProductTopSellQuestion + [0] * sizeRecommendedProductQuestion + [0] * sizeIngredientQuestion + [0] * sizeQualityQuestion + [0] * sizeGreetingQuestion
        recommendedProducts = [0] * sizeInformationQuestion + [0] * sizeProductTopSellQuestion + [1] * sizeRecommendedProductQuestion + [0] * sizeIngredientQuestion + [0] * sizeQualityQuestion + [0] * sizeGreetingQuestion
        ingredientQuestions = [0] * sizeInformationQuestion + [0] * sizeProductTopSellQuestion + [0] * sizeRecommendedProductQuestion + [1] * sizeIngredientQuestion + [0] * sizeQualityQuestion + [0] * sizeGreetingQuestion
        qualityQuestions = [0] * sizeInformationQuestion + [0] * sizeProductTopSellQuestion + [0] * sizeRecommendedProductQuestion + [0] * sizeIngredientQuestion + [1] * sizeQualityQuestion + [0] * sizeGreetingQuestion
        greetingQuestions = [0] * sizeInformationQuestion + [0] * sizeProductTopSellQuestion + [0] * sizeRecommendedProductQuestion + [0] * sizeIngredientQuestion + [0] * sizeQualityQuestion + [1] * sizeGreetingQuestion
        

        # Tạo DataFrame từ dữ liệu
        data = {'Question': questions,
                'infomation': infomations,
                'productTopSell': productTopSells,
                'recommendedProduct': recommendedProducts,
                'ingredient': ingredientQuestions,
                'quality' : qualityQuestions,
                'greeting': greetingQuestions}

        df = pd.DataFrame(data)
        df = df.sample(frac=1).reset_index(drop=True)

        directory = 'Data'
        if not os.path.exists(directory):
            os.makedirs(directory)

        df.to_csv(os.path.join(directory, 'coffee.csv'), index=False)

        print("Xuất ra thành công!")

cd = CreateData()
cd.fit()
cd.exportCsv()