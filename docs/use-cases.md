# Use Cases - Bo Use Case Nghiep Vu Cua He Thong

## 1. Muc dich tai lieu

Tai lieu nay trinh bay he thong duoi dang `Use Case`.

Muc tieu:

- giup ban hieu he thong theo cach business analyst thuong viet
- chuan hoa actor, precondition, main flow, alternate flow, exception flow
- giup chuyen tu "moi man hinh co gi" sang "nguoi dung dang giai quyet viec gi"

Tai lieu nay rat huu ich khi:

- phan tich yeu cau
- viet backlog
- nghiem thu
- trao doi voi stakeholder

## 2. Danh sach actor

He thong hien tai co 3 actor chinh:

- `Admin`
- `WarehouseStaff`
- `Viewer`

## 3. Danh sach use case tong hop

1. Dang nhap he thong
2. Dang xuat he thong
3. Xem dashboard ton kho
4. Xem danh sach category
5. Tao category
6. Sua category
7. Xoa category
8. Xem danh sach product
9. Tao product
10. Sua product
11. Xoa product
12. Xem lich su receipt
13. Xem chi tiet receipt
14. Tao receipt
15. Xem lich su issue
16. Xem chi tiet issue
17. Tao issue

## 4. Use Case 01 - Dang nhap he thong

### Muc tieu

Cho phep nguoi dung xac thuc de su dung he thong.

### Actor chinh

- Admin
- WarehouseStaff
- Viewer

### Tien dieu kien

- User da ton tai trong he thong
- User co username/email va password hop le

### Kich hoat

- User truy cap he thong va can dang nhap

### Main flow

1. User mo man login
2. User nhap username/email
3. User nhap password
4. He thong xac thuc thong tin
5. He thong tao session
6. He thong tra role cua user
7. User duoc chuyen vao he thong

### Alternate flow

1. User nhap email thay vi username
2. He thong tim thay user theo email
3. Tiep tuc xac thuc password

### Exception flow

1. User nhap sai thong tin
2. He thong tu choi dang nhap
3. He thong thong bao sai username/email hoac password

### Hau dieu kien

- Thanh cong:
  - user da dang nhap
  - UI biet role cua user
- That bai:
  - user van o trang thai chua dang nhap

## 5. Use Case 02 - Dang xuat he thong

### Muc tieu

Ket thuc phien lam viec hien tai.

### Actor chinh

- Moi user da dang nhap

### Tien dieu kien

- User dang co session hop le

### Main flow

1. User nhan nut logout
2. He thong huy session
3. User duoc dua ve trang login

### Hau dieu kien

- Session khong con hop le
- User phai dang nhap lai neu muon vao he thong

## 6. Use Case 03 - Xem dashboard ton kho

### Muc tieu

Cho user xem tong quan tinh hinh kho hien tai.

### Actor chinh

- Admin
- WarehouseStaff
- Viewer

### Tien dieu kien

- User da dang nhap

### Main flow

1. User vao dashboard
2. He thong lay inventory summary
3. He thong hien:
   - total products
   - total on hand units
   - total inventory value
   - low stock count
   - low stock items

### Hau dieu kien

- User co buc tranh tong quan de ra quyet dinh

## 7. Use Case 04 - Xem danh sach category

### Muc tieu

Xem cac nhom san pham hien co.

### Actor chinh

- Admin
- WarehouseStaff
- Viewer

### Tien dieu kien

- User da dang nhap

### Main flow

1. User vao man categories
2. He thong tai danh sach category
3. He thong hien ten, mo ta, ngay tao

### Hau dieu kien

- User hieu danh muc nhom hang hien co

## 8. Use Case 05 - Tao category

### Muc tieu

Tao nhom san pham moi.

### Actor chinh

- Admin

### Tien dieu kien

- User da dang nhap voi role Admin
- Ten category chua bi trung

### Main flow

1. Admin vao man categories
2. Admin nhap ten va mo ta
3. Admin nhan create
4. He thong validate du lieu
5. He thong luu category moi
6. He thong hien category trong danh sach

### Exception flow

1. Ten category bi trung
2. He thong tu choi tao
3. He thong hien thong bao loi

### Hau dieu kien

- Category moi co san de tao product

## 9. Use Case 06 - Sua category

### Muc tieu

Cap nhat thong tin category da ton tai.

### Actor chinh

- Admin

### Tien dieu kien

- User la Admin
- Category ton tai

### Main flow

1. Admin chon category can sua
2. He thong hien du lieu hien tai
3. Admin cap nhat ten/mo ta
4. He thong validate
5. He thong luu thay doi

### Exception flow

1. Ten moi bi trung voi category khac
2. He thong tu choi luu

## 10. Use Case 07 - Xoa category

### Muc tieu

Xoa category khong con duoc su dung.

### Actor chinh

- Admin

### Tien dieu kien

- User la Admin
- Category ton tai

### Main flow

1. Admin chon xoa category
2. He thong kiem tra category co product khong
3. Neu khong co product:
   - he thong xoa category

### Exception flow

1. Category dang duoc product tham chieu
2. He thong tu choi xoa

### Hau dieu kien

- Category bi xoa khoi danh muc neu hop le

## 11. Use Case 08 - Xem danh sach product

### Muc tieu

Xem danh muc mat hang dang duoc quan ly.

### Actor chinh

- Admin
- WarehouseStaff
- Viewer

### Main flow

1. User vao man products
2. He thong tai danh sach product
3. He thong hien:
   - SKU
   - Name
   - Category
   - OnHandQty
   - AverageCost
   - ReorderLevel
   - IsActive

### Hau dieu kien

- User biet tinh hinh danh muc hang hoa

## 12. Use Case 09 - Tao product

### Muc tieu

Tao mat hang moi de dua vao he thong kho.

### Actor chinh

- Admin

### Tien dieu kien

- Admin da dang nhap
- Category ton tai
- SKU chua bi trung

### Main flow

1. Admin vao man products
2. Nhap SKU, Name, Description, Category, ReorderLevel
3. Nhan create
4. He thong validate
5. He thong tao product moi voi:
   - OnHandQty = 0
   - AverageCost = 0
   - IsActive = true

### Exception flow

1. Category khong ton tai
2. SKU bi trung
3. He thong tu choi tao

## 13. Use Case 10 - Sua product

### Muc tieu

Cap nhat thong tin nghiep vu cua product.

### Actor chinh

- Admin

### Tien dieu kien

- Product ton tai
- Category ton tai

### Main flow

1. Admin chon product can sua
2. He thong nap du lieu
3. Admin cap nhat thong tin
4. He thong validate SKU/category
5. He thong luu thay doi

### Hau dieu kien

- Product duoc cap nhat dung theo nghiep vu moi

## 14. Use Case 11 - Xoa product

### Muc tieu

Xoa product neu chua co lich su nghiep vu.

### Actor chinh

- Admin

### Tien dieu kien

- Product ton tai

### Main flow

1. Admin nhan xoa product
2. He thong kiem tra product co transaction history khong
3. Neu khong co:
   - he thong xoa

### Exception flow

1. Product da co receipt line hoac issue line
2. He thong tu choi xoa

### Hau dieu kien

- Product chi duoc xoa khi an toan nghiep vu

## 15. Use Case 12 - Xem lich su receipt

### Muc tieu

Xem cac lan nhap kho da xay ra.

### Actor chinh

- Admin
- WarehouseStaff
- Viewer

### Main flow

1. User vao man receipts
2. He thong tai danh sach receipts
3. He thong hien doc no, date, supplier, total, line count

### Hau dieu kien

- User co the theo doi lich su nhap kho

## 16. Use Case 13 - Xem chi tiet receipt

### Muc tieu

Xem chi tiet tung phieu nhap kho.

### Actor chinh

- Admin
- WarehouseStaff
- Viewer

### Main flow

1. User chon receipt
2. He thong hien dau phieu
3. He thong hien cac line:
   - product
   - quantity
   - unit cost
   - line total

### Hau dieu kien

- User co the doi chieu receipt cu

## 17. Use Case 14 - Tao receipt

### Muc tieu

Nhap hang vao kho.

### Actor chinh

- Admin
- WarehouseStaff

### Tien dieu kien

- Product ton tai va active
- Quantity > 0
- UnitCost >= 0
- Co it nhat 1 line

### Main flow

1. User vao man receipts
2. Nhap supplier, note
3. Chon product
4. Nhap quantity va unit cost
5. Them line
6. Lap lai neu can nhieu line
7. Nhan submit
8. He thong validate request
9. He thong cap nhat ton
10. He thong tinh average cost moi
11. He thong tao ledger
12. He thong luu receipt

### Alternate flow

1. Receipt co nhieu line cho nhieu product
2. He thong xu ly tung line trong cung 1 giao dich

### Exception flow

1. Product inactive hoac khong ton tai
2. Quantity khong hop le
3. Unit cost am
4. He thong tu choi giao dich

### Hau dieu kien

- Ton kho tang
- Gia tri ton kho thay doi
- Co phieu nhap moi trong history

## 18. Use Case 15 - Xem lich su issue

### Muc tieu

Xem cac lan xuat kho da xay ra.

### Actor chinh

- Admin
- WarehouseStaff
- Viewer

### Main flow

1. User vao man issues
2. He thong tai danh sach issues
3. Hien doc no, date, customer, total, line count

## 19. Use Case 16 - Xem chi tiet issue

### Muc tieu

Xem chi tiet tung phieu xuat kho.

### Actor chinh

- Admin
- WarehouseStaff
- Viewer

### Main flow

1. User chon issue
2. He thong hien dau phieu
3. Hien cac line xuat

### Hau dieu kien

- User co the doi chieu nghiep vu xuat kho

## 20. Use Case 17 - Tao issue

### Muc tieu

Xuat hang ra kho.

### Actor chinh

- Admin
- WarehouseStaff

### Tien dieu kien

- Co it nhat 1 line
- Product ton tai va active
- Quantity > 0
- Ton kho du

### Main flow

1. User vao man issues
2. Nhap customer, note
3. Chon product
4. Nhap quantity
5. Them line
6. Nhan submit
7. He thong validate ton kho
8. He thong lay average cost hien tai lam unit cost xuat
9. He thong giam ton
10. He thong tao ledger
11. He thong luu issue

### Exception flow

1. Product khong ton tai
2. Product inactive
3. Ton khong du
4. He thong tu choi giao dich

### Hau dieu kien

- Ton kho giam
- Co phieu xuat moi
- Co lich su truy vet

## 21. Use case va role matrix

| Use case | Admin | WarehouseStaff | Viewer |
|---|---|---|---|
| Dang nhap | Co | Co | Co |
| Dang xuat | Co | Co | Co |
| Xem dashboard | Co | Co | Co |
| Xem categories | Co | Co | Co |
| Tao/sua/xoa category | Co | Khong | Khong |
| Xem products | Co | Co | Co |
| Tao/sua/xoa product | Co | Khong | Khong |
| Xem receipts | Co | Co | Co |
| Tao receipt | Co | Co | Khong |
| Xem issues | Co | Co | Co |
| Tao issue | Co | Co | Khong |

## 22. Ket luan

Use case giup ban nhin he thong khong theo file, khong theo table, ma theo:

- nguoi nao
- muon lam viec gi
- vi gia tri business nao

Day la cach rat tot de hoc business analysis tren mot du an phan mem that.
