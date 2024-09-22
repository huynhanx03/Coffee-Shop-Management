import firebase_admin
from firebase_admin import credentials
from firebase_admin import db

# Khởi tạo kết nối với Firebase
cred = credentials.Certificate('serviceAccountKey.json')
firebase_admin.initialize_app(cred, {
    'databaseURL': 'https://coffee-4053c-default-rtdb.asia-southeast1.firebasedatabase.app/'
})

def GetProduct():
    # Lấy dữ liệu từ nút "SanPham" trong Firebase
    product_data = db.reference('SanPham').get()

    # Lấy dữ liệu từ nút "LoaiSanPham" trong Firebase
    product_type_data = db.reference('LoaiSanPham').get()
    list_product_type = list(product_type_data.values())

    # Xử lý dữ liệu
    products = [
        {
            'MaSanPham': product['MaSanPham'],
            'TenSanPham': product['TenSanPham'],
            'SoLuong': product['SoLuong'],
            'LoaiSanPham': next((x['LoaiSanPham'] for x in list_product_type if x['MaLoaiSanPham'] == product['MaLoaiSanPham']), None),
            'CongThuc': ' | '.join([congthuc['TenNguyenLieu'] for congthuc in product['CongThuc'].values()])
        }
        for product in product_data.values()
    ]

    return products

def GetEvaluate():
    evaluate_data = db.reference('DanhGia').get()

    evaluates = [
        {
            'MaKhachHang': evaluete['MaNguoiDung'],
            'MaSanPham': evaluete['MaSanPham'],
            'DiemDanhGia': evaluete['DiemDanhGia'] 
        }
        for evaluete in evaluate_data.values() 
    ]

    return evaluates