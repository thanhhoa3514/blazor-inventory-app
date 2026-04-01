# API Business Mapping - Map API Sang Nghiep Vu

## 1. Muc dich tai lieu

Tai lieu nay duoc viet de giup ban hieu:

- moi API trong he thong co y nghia nghiep vu gi
- request field va response field phuc vu bai toan nao
- API do duoc goi tu man hinh nao
- actor nao duoc phep goi
- business validation nao dang duoc server thuc hien
- loi nghiep vu se xuat hien trong truong hop nao

Neu hoc business ma chi nhin UI, ban se thay "man hinh".  
Neu hoc business ma chi nhin code, ban se thay "method".  
Tai lieu nay noi hai the gioi do lai bang `use case`.

## 2. Nguyen tac doc tai lieu nay

Moi API duoc mo ta theo khung:

1. `Muc dich nghiep vu`
2. `Actor`
3. `Tien dieu kien`
4. `Input business meaning`
5. `Xu ly nghiep vu`
6. `Ket qua thanh cong`
7. `That bai nghiep vu co the gap`
8. `Man hinh lien quan`

## 3. Nhom API Authentication

### 3.1. `POST /api/auth/login`

#### Muc dich nghiep vu

Xac thuc nguoi dung de he thong biet ai dang thao tac.

#### Actor

- Admin
- WarehouseStaff
- Viewer

#### Tien dieu kien

- Tai khoan da ton tai
- Username/Email va password hop le

#### Input business meaning

- `UserNameOrEmail`: danh tinh nguoi dung
- `Password`: thong tin xac thuc

#### Xu ly nghiep vu

1. Tim user theo username hoac email
2. Kiem tra password
3. Neu dung:
   - tao phien dang nhap
   - tra ve thong tin session va role

#### Ket qua thanh cong

- user duoc dang nhap
- cookie session duoc tao
- client biet role cua user

#### That bai nghiep vu

- sai user hoac sai password
- user khong ton tai

#### Man hinh lien quan

- `/login`

### 3.2. `POST /api/auth/logout`

#### Muc dich nghiep vu

Ket thuc phien lam viec cua nguoi dung.

#### Actor

- bat ky user da dang nhap nao

#### Tien dieu kien

- user dang co session hop le

#### Xu ly nghiep vu

- xoa session xac thuc

#### Ket qua thanh cong

- user quay ve trang thai chua dang nhap

#### Man hinh lien quan

- nut logout tren shell

### 3.3. `GET /api/auth/me`

#### Muc dich nghiep vu

Cho client biet user hien tai la ai va co role nao.

#### Actor

- user da dang nhap

#### Xu ly nghiep vu

- doc user tu session hien tai
- lay role cua user
- tra ve session dto

#### Ket qua business

- UI biet de hien thi menu, nut, form theo role

## 4. Nhom API Categories

### 4.1. `GET /api/categories`

#### Muc dich nghiep vu

Lay danh sach category hien co trong he thong.

#### Actor

- Admin
- WarehouseStaff
- Viewer

#### Business meaning

API nay phuc vu:

- xem danh muc nhom hang
- chon category khi tao product
- phan tich nhom san pham

#### Xu ly nghiep vu

- doc toan bo category
- sap xep theo ten

#### Ket qua thanh cong

- tra ve danh sach category cho UI

#### Man hinh lien quan

- `/categories`
- `/products`

### 4.2. `GET /api/categories/{id}`

#### Muc dich nghiep vu

Lay chi tiet mot category cu the.

#### Actor

- tat ca user da dang nhap

#### Khi dung

- khi can nap du lieu chi tiet theo id
- khi mo rong he thong ve sau

#### Ket qua that bai

- `404` neu category khong ton tai

### 4.3. `POST /api/categories`

#### Muc dich nghiep vu

Tao category moi de phan loai san pham.

#### Actor

- chi `Admin`

#### Tien dieu kien

- ten category hop le
- ten category chua ton tai

#### Input business meaning

- `Name`: ten nhom san pham
- `Description`: mo ta nghiep vu cua nhom

#### Xu ly nghiep vu

1. validate model
2. trim ten va mo ta
3. kiem tra trung ten
4. tao category moi

#### Ket qua thanh cong

- category moi duoc dua vao he thong

#### That bai nghiep vu

- ten trong
- ten vuot do dai
- ten bi trung

#### Man hinh lien quan

- `/categories`

### 4.4. `PUT /api/categories/{id}`

#### Muc dich nghiep vu

Cap nhat thong tin category da co.

#### Actor

- chi `Admin`

#### Tien dieu kien

- category ton tai
- ten moi khong bi trung voi category khac

#### Xu ly nghiep vu

1. tim category theo id
2. neu khong co -> 404
3. kiem tra trung ten
4. cap nhat du lieu

#### Gia tri business

- giup category duoc chuan hoa lai khi mo ta chua dung hoac can doi ten

### 4.5. `DELETE /api/categories/{id}`

#### Muc dich nghiep vu

Xoa category khi no chua duoc su dung boi product nao.

#### Actor

- chi `Admin`

#### Tien dieu kien

- category ton tai
- category khong co product nao tham chieu

#### Xu ly nghiep vu

1. tim category
2. neu khong co -> 404
3. kiem tra co product nao thuoc category khong
4. neu co -> tu choi xoa
5. neu khong -> xoa

#### That bai nghiep vu

- `400`: category dang duoc su dung boi product

#### Ly do business

- de giu toan ven master data

## 5. Nhom API Products

### 5.1. `GET /api/products`

#### Muc dich nghiep vu

Lay danh sach san pham cung thong tin ton hien tai.

#### Actor

- Admin
- WarehouseStaff
- Viewer

#### Business meaning

API nay la nguon du lieu chinh de:

- xem product catalog
- xem on hand qty
- xem average cost
- xem reorder level
- chon product khi tao receipt/issue

#### Xu ly nghiep vu

- doc product
- join sang category
- sap xep theo ten
- tra ve thong tin tong hop can thiet cho UI

#### Gia tri kinh doanh

- cho biet tai san hang hoa dang co trong he thong

### 5.2. `GET /api/products/{id}`

#### Muc dich nghiep vu

Lay chi tiet mot product.

#### Actor

- tat ca user da dang nhap

#### Xu ly nghiep vu

- tim product theo id
- join category

#### That bai nghiep vu

- `404`: product khong ton tai

### 5.3. `POST /api/products`

#### Muc dich nghiep vu

Tao moi mot mat hang trong kho.

#### Actor

- chi `Admin`

#### Tien dieu kien

- SKU hop le
- Name hop le
- Category ton tai
- SKU chua duoc dung

#### Input business meaning

- `Sku`: ma nghiep vu cua mat hang
- `Name`: ten san pham
- `Description`: mo ta san pham
- `CategoryId`: nhom san pham
- `ReorderLevel`: nguong canh bao sap het

#### Xu ly nghiep vu

1. validate request
2. kiem tra category ton tai
3. chuan hoa SKU bang cach trim va upper-case
4. kiem tra trung SKU
5. tao product voi ton ban dau = 0

#### Ket qua thanh cong

- product moi ton tai trong he thong
- co the dua vao giao dich nhap kho

#### That bai nghiep vu

- category khong ton tai
- SKU trung
- du lieu khong hop le

### 5.4. `PUT /api/products/{id}`

#### Muc dich nghiep vu

Cap nhat product da co.

#### Actor

- chi `Admin`

#### Tien dieu kien

- product ton tai
- category ton tai
- SKU moi khong trung voi product khac

#### Xu ly nghiep vu

1. tim product theo id
2. kiem tra category
3. kiem tra SKU unique
4. cap nhat:
   - Sku
   - Name
   - Description
   - CategoryId
   - ReorderLevel
   - IsActive
   - LastUpdatedUtc

#### Gia tri business

- cho phep dieu chinh master data khi doi ten, doi nhom, doi nguong dat hang lai, hoac tam ngung san pham

### 5.5. `DELETE /api/products/{id}`

#### Muc dich nghiep vu

Xoa product neu chua co lich su giao dich.

#### Actor

- chi `Admin`

#### Tien dieu kien

- product ton tai
- khong co stock receipt line
- khong co stock issue line

#### Xu ly nghiep vu

1. tim product
2. neu khong co -> 404
3. kiem tra product da xuat hien trong giao dich nao chua
4. neu da co -> tu choi
5. neu chua -> xoa

#### That bai nghiep vu

- `400`: product co transaction history

#### Business rationale

- dam bao lich su kho khong bi mat nghia

## 6. Nhom API Receipts

### 6.1. `GET /api/receipts`

#### Muc dich nghiep vu

Lay lich su phieu nhap kho.

#### Actor

- Admin
- WarehouseStaff
- Viewer

#### Business meaning

Phuc vu:

- xem cac dot nhap hang
- doi chieu supplier
- theo doi tong gia tri nhap

#### Xu ly nghiep vu

- doc stock receipts
- sap xep moi nhat len truoc
- tra ve thong tin list

#### Man hinh lien quan

- `/receipts`

### 6.2. `GET /api/receipts/{id}`

#### Muc dich nghiep vu

Lay chi tiet mot phieu nhap.

#### Actor

- tat ca user da dang nhap

#### Business meaning

Phuc vu:

- xem tung line da nhap gi
- xem gia nhap theo tung line
- doi chieu receipt khi can

#### Xu ly nghiep vu

- load dau phieu
- load lines
- load product cho tung line

#### That bai nghiep vu

- `404`: khong tim thay phieu

### 6.3. `POST /api/receipts`

#### Muc dich nghiep vu

Tao phieu nhap kho moi.

#### Actor

- `Admin`
- `WarehouseStaff`

#### Tien dieu kien

- co it nhat 1 line
- moi line co product hop le
- product dang active
- quantity > 0
- unit cost >= 0

#### Input business meaning

- `Supplier`: nha cung cap hoac nguon nhap
- `Note`: ghi chu chung
- `Lines`: danh sach hang nhap

Moi line:

- `ProductId`: hang duoc nhap
- `Quantity`: so luong vao kho
- `UnitCost`: gia nhap cua line do

#### Xu ly nghiep vu

1. validate request
2. tao document no
3. mo database transaction
4. lap qua tung line:
   - kiem tra product active
   - tinh line total
   - tinh current inventory value
   - tinh new qty
   - tinh new average cost
   - cap nhat product
   - tao receipt line
   - tao ledger entry
5. tinh total amount cua receipt
6. save changes
7. commit transaction
8. tra ve receipt detail

#### Ket qua thanh cong

- ton kho tang
- average cost duoc cap nhat
- co phieu nhap moi trong history
- co ledger entry de truy vet

#### That bai nghiep vu

- product khong ton tai
- product inactive
- quantity <= 0
- unit cost am
- request khong co line

#### Business impact

- day la diem vao cua inventory asset

## 7. Nhom API Issues

### 7.1. `GET /api/issues`

#### Muc dich nghiep vu

Lay lich su phieu xuat kho.

#### Actor

- Admin
- WarehouseStaff
- Viewer

#### Business meaning

Phuc vu:

- xem luong hang da ra kho
- xem customer/noi nhan
- xem tong gia tri issue

### 7.2. `GET /api/issues/{id}`

#### Muc dich nghiep vu

Lay chi tiet mot phieu xuat.

#### Actor

- tat ca user da dang nhap

#### Business meaning

- biet da xuat mat hang nao
- bao nhieu
- theo gia tri nao

### 7.3. `POST /api/issues`

#### Muc dich nghiep vu

Tao phieu xuat kho moi.

#### Actor

- `Admin`
- `WarehouseStaff`

#### Tien dieu kien

- co it nhat 1 line
- product ton tai va active
- quantity > 0
- ton kho du de xuat

#### Input business meaning

- `Customer`: noi nhan/khach hang
- `Note`: ghi chu nghiep vu
- `Lines`: danh sach san pham xuat

#### Xu ly nghiep vu

1. validate request
2. tao document no
3. mo database transaction
4. lap qua tung line:
   - lay product
   - kiem tra ton kho du
   - lay average cost lam unit cost xuat
   - tinh line total
   - giam on hand qty
   - tao issue line
   - tao ledger entry voi quantity am
5. tinh total amount
6. luu va commit
7. tra ve issue detail

#### Ket qua thanh cong

- ton kho giam
- co phieu xuat moi
- co ledger entry

#### That bai nghiep vu

- request khong co line
- quantity <= 0
- product inactive
- product khong ton tai
- ton kho khong du

#### Business impact

- day la diem ra cua inventory asset

## 8. Nhom API Inventory Summary

### 8.1. `GET /api/inventory/summary`

#### Muc dich nghiep vu

Lay buc tranh tong hop ton kho hien tai.

#### Actor

- Admin
- WarehouseStaff
- Viewer

#### Business meaning

API nay duoc dung de tra loi nhanh:

- hien co bao nhieu san pham dang active
- tong don vi ton kho la bao nhieu
- tong gia tri ton kho la bao nhieu
- co bao nhieu mat hang sap het
- cu the mat hang nao sap het

#### Xu ly nghiep vu

1. doc toan bo product active
2. tinh:
   - total products
   - total on hand units
   - total inventory value
   - low stock count
   - low stock items

#### Ket qua business

- phuc vu dashboard va quyet dinh nhap them

## 9. Mapping API sang use case nghiep vu

| Use case | API chinh |
|---|---|
| Dang nhap he thong | `POST /api/auth/login` |
| Xem user hien tai | `GET /api/auth/me` |
| Dang xuat | `POST /api/auth/logout` |
| Xem danh muc category | `GET /api/categories` |
| Tao category | `POST /api/categories` |
| Sua category | `PUT /api/categories/{id}` |
| Xoa category | `DELETE /api/categories/{id}` |
| Xem danh muc product | `GET /api/products` |
| Tao product | `POST /api/products` |
| Sua product | `PUT /api/products/{id}` |
| Xoa product | `DELETE /api/products/{id}` |
| Xem lich su receipt | `GET /api/receipts` |
| Xem chi tiet receipt | `GET /api/receipts/{id}` |
| Tao receipt | `POST /api/receipts` |
| Xem lich su issue | `GET /api/issues` |
| Xem chi tiet issue | `GET /api/issues/{id}` |
| Tao issue | `POST /api/issues` |
| Xem dashboard ton kho | `GET /api/inventory/summary` |

## 10. Mapping API sang role

### 10.1. Admin

Goi duoc:

- toan bo API nghiep vu hien tai

### 10.2. WarehouseStaff

Goi duoc:

- `GET` category, product, receipt, issue, inventory summary
- `POST /api/receipts`
- `POST /api/issues`

Khong goi duoc:

- `POST/PUT/DELETE` categories
- `POST/PUT/DELETE` products

### 10.3. Viewer

Goi duoc:

- tat ca API chi doc

Khong goi duoc:

- tat ca API tao/sua/xoa nghiep vu

## 11. Business validation dang nam o dau

Trong du an nay, validation nghiep vu dang nam o 3 lop:

### 11.1. Data annotation

Dung cho:

- required
- max length
- min length
- range

Day la validation "hinh thuc du lieu".

### 11.2. Controller validation

Dung cho:

- category co ton tai khong
- SKU co trung khong
- duoc phep thao tac role nao

Day la validation "master data va permission".

### 11.3. Service validation

Dung cho:

- receipt phai co line
- issue phai co line
- khong duoc xuat vuot ton
- khong duoc giao dich voi product inactive
- cap nhat average cost

Day la validation "nghiep vu giao dich".

Khi hoc business system, can phan biet ro 3 tang validation nay.

## 12. Goc nhin hoc business tu API

Neu ban muon hoc business khong bi roi, hay doc API theo thu tu:

1. auth
2. categories
3. products
4. receipts
5. issues
6. inventory summary

Ly do:

- auth cho biet ai duoc dung he thong
- categories va products cho biet doi tuong duoc quan ly
- receipts va issues cho biet nghiep vu van hanh
- summary cho biet gia tri tong hop cuoi cung

## 13. Ket luan

Moi API trong he thong nay khong chi la "endpoint".  
No la mot "dong tac nghiep vu" cua don vi van hanh kho.

Tom gon:

- auth API xac dinh nguoi dung va quyen
- category/product API quan ly master data
- receipt/issue API tao bien dong kho
- inventory summary API tong hop ket qua kinh doanh cua cac bien dong do

Khi ban doc API theo goc nhin nay, ban se thay du an khong con la CRUD thuan tuy ma la mot mo hinh nghiep vu co trach nhiem, quy tac va dong chay du lieu ro rang.
