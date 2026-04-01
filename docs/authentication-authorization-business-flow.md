# Tai Lieu Nghiep Vu Authentication va Authorization

## 1. Muc dich tai lieu

Tai lieu nay mo ta ro nghiep vu `Authentication` va `Authorization` trong he thong quan ly kho `MyApp Inventory`.

Muc tieu la:

- Giai thich vi sao he thong kho bat buoc phai co dang nhap va phan quyen.
- Mo ta ro tung vai tro nghiep vu trong kho.
- Chi ra ai duoc xem gi, ai duoc thao tac gi, ai bi chan gi.
- Trinh bay toan bo luong hoat dong tu luc nguoi dung dang nhap den luc tao giao dich nhap kho, xuat kho, quan tri danh muc.
- Gan nghiep vu voi cac man hinh va endpoint hien tai de de theo doi trong code.

Tai lieu nay uu tien goc nhin nghiep vu, nhung van lien ket truc tiep voi cach he thong da duoc trien khai.

## 2. Tong quan nghiep vu kho

He thong hien tai phuc vu bai toan quan ly kho co ban, gom cac nhom chuc nang sau:

- Quan ly `Category`
- Quan ly `Product`
- Lap `Stock Receipt` de nhap kho
- Lap `Stock Issue` de xuat kho
- Xem `Inventory Summary` va lich su giao dich

Ban chat cua he thong la:

- `Product` la mat hang ton kho.
- `Category` la nhom phan loai cua san pham.
- `Receipt` lam tang ton kho.
- `Issue` lam giam ton kho.
- `Inventory Summary` tong hop ton hien tai va canh bao thieu hang.

Trong nghiep vu kho, khong phai ai cung duoc tao hoac sua moi giao dich. Vi du:

- Nhan vien xem bao cao khong duoc xuat hang.
- Nhan vien kho duoc nhap xuat hang nhung khong duoc xoa san pham.
- Quan tri vien duoc phep cau hinh danh muc va xu ly thao tac nhay cam.

Do do, he thong phai co:

- `Authentication`: xac dinh nguoi dang su dung he thong la ai.
- `Authorization`: xac dinh nguoi do duoc phep lam gi trong he thong.

## 3. Dinh nghia nghiep vu

### 3.1. Authentication la gi

`Authentication` la qua trinh xac thuc danh tinh nguoi dung.

Trong he thong nay, nguoi dung dang nhap bang:

- `Username` hoac `Email`
- `Password`

Sau khi dang nhap thanh cong:

- Server tao phien dang nhap.
- Trinh duyet giu cookie xac thuc.
- Cac request tiep theo duoc xem la request cua nguoi dung da dang nhap.

Neu chua dang nhap:

- Nguoi dung khong duoc truy cap cac man hinh chinh.
- Goi API nghiep vu se bi tra ve `401 Unauthorized`.

### 3.2. Authorization la gi

`Authorization` la qua trinh kiem tra quyen.

Sau khi he thong biet nguoi dung la ai, he thong tiep tuc xac dinh:

- Nguoi do thuoc vai tro nao.
- Vai tro do duoc phep thuc hien thao tac nao.

Trong he thong nay, `Authorization` duoc thuc hien bang `RBAC - Role-Based Access Control`.

Nghia la:

- Quyen duoc cap theo `role`.
- Nguoi dung co mot hoac nhieu role.
- He thong kiem tra role de cho phep hoac tu choi thao tac.

## 4. Muc tieu nghiep vu cua RBAC trong quan ly kho

RBAC trong he thong kho khong chi de "bao mat". Muc tieu nghiep vu cua no la:

1. Giam sai sot van hanh.
2. Ngan thay doi trai phep tren du lieu ton kho.
3. Bao ve thao tac nhay cam nhu xoa san pham, sua danh muc, nhap kho, xuat kho.
4. Tach bach trach nhiem giua nguoi quan tri, nhan vien kho, va nguoi chi xem bao cao.
5. Tao nen tang cho audit trail va truy vet sau nay.

## 5. Vai tro nghiep vu trong he thong

He thong hien tai dinh nghia 3 vai tro:

- `Admin`
- `WarehouseStaff`
- `Viewer`

### 5.1. Admin

`Admin` la nguoi quan tri he thong va du lieu danh muc.

Trach nhiem nghiep vu:

- Tao, sua, xoa category.
- Tao, sua, xoa product.
- Xem tat ca du lieu kho.
- Co the thuc hien giao dich nhap kho, xuat kho.
- Co toan quyen tren he thong hien tai.

Admin la vai tro co quyen cao nhat.

### 5.2. WarehouseStaff

`WarehouseStaff` la nhan vien van hanh kho.

Trach nhiem nghiep vu:

- Xem danh muc san pham va tinh hinh ton kho.
- Lap phieu nhap kho.
- Lap phieu xuat kho.
- Xem lich su giao dich.

WarehouseStaff khong duoc:

- Tao, sua, xoa category.
- Tao, sua, xoa product.
- Thay doi cau truc du lieu danh muc.

Ly do nghiep vu:

- Nhan vien kho chi nen xu ly giao dich van hanh.
- Danh muc master data phai duoc kiem soat boi Admin.

### 5.3. Viewer

`Viewer` la nguoi chi duoc xem.

Doi tuong nghiep vu co the la:

- Quan ly xem bao cao.
- Ke toan can doi so lieu.
- Nhan vien bo phan lien quan can theo doi ton kho.

Viewer duoc:

- Xem dashboard.
- Xem products, categories.
- Xem receipts, issues, inventory summary.

Viewer khong duoc:

- Tao receipt.
- Tao issue.
- Tao hoac xoa product.
- Tao hoac xoa category.

## 6. Ma tran quyen nghiep vu

Bang duoi day mo ta quyen theo nghiep vu:

| Chuc nang | Admin | WarehouseStaff | Viewer |
|---|---|---|---|
| Dang nhap he thong | Co | Co | Co |
| Xem dashboard ton kho | Co | Co | Co |
| Xem danh sach category | Co | Co | Co |
| Tao category | Co | Khong | Khong |
| Sua category | Co | Khong | Khong |
| Xoa category | Co | Khong | Khong |
| Xem danh sach product | Co | Co | Co |
| Tao product | Co | Khong | Khong |
| Sua product | Co | Khong | Khong |
| Xoa product | Co | Khong | Khong |
| Xem receipt history | Co | Co | Co |
| Tao receipt | Co | Co | Khong |
| Xem issue history | Co | Co | Co |
| Tao issue | Co | Co | Khong |
| Xem inventory summary | Co | Co | Co |

## 7. Quy tac nghiep vu cot loi

### 7.1. Quy tac ve truy cap he thong

- Moi man hinh nghiep vu chinh deu yeu cau nguoi dung phai dang nhap.
- Neu chua dang nhap, nguoi dung phai duoc dua ve man login hoac nhan thong bao chua duoc xac thuc.

### 7.2. Quy tac ve xem du lieu

- Moi role da dang nhap deu duoc xem du lieu tong quan kho.
- Viec "xem" du lieu duoc coi la nhu cau van hanh co ban va co the cap cho `Viewer`.

### 7.3. Quy tac ve giao dich kho

- Tao `Receipt` va `Issue` la thao tac lam bien dong ton kho.
- Chi `Admin` va `WarehouseStaff` moi duoc phep tao cac giao dich nay.
- `Viewer` bi cam tao giao dich vi khong tham gia van hanh truc tiep.

### 7.4. Quy tac ve master data

- `Category` va `Product` la du lieu master.
- Du lieu master anh huong den toan he thong.
- Chi `Admin` duoc phep tao, sua, xoa master data.

### 7.5. Quy tac ve thao tac nhay cam

Thao tac nhay cam bao gom:

- Xoa product
- Xoa category
- Sua product
- Sua category
- Tao giao dich nhap kho
- Tao giao dich xuat kho

Nguyen tac kiem soat:

- Thao tac xoa/sua du lieu master phai duoc gioi han chat che.
- Thao tac lam thay doi ton kho chi duoc cap cho role co trach nhiem van hanh.

## 8. Luong nghiep vu tong the

### 8.1. Luong 1 - Dang nhap vao he thong

1. Nguoi dung truy cap he thong.
2. He thong kiem tra xem da co phien dang nhap hop le hay chua.
3. Neu chua co:
   - Hien man `Login`.
4. Nguoi dung nhap `Username/Email` va `Password`.
5. Server xac thuc thong tin.
6. Neu dung:
   - Tao phien dang nhap.
   - Tra thong tin user va role.
   - Cho phep vao he thong.
7. Neu sai:
   - Bao loi dang nhap.
   - Khong tao phien.

Y nghia nghiep vu:

- Moi thao tac tiep theo deu gan voi mot nguoi dung da xac dinh.

### 8.2. Luong 2 - Nguoi dung xem dashboard ton kho

1. Nguoi dung da dang nhap vao he thong.
2. Truy cap dashboard.
3. He thong kiem tra role co thuoc nhom duoc xem du lieu hay khong.
4. Neu co:
   - Hien `Inventory Summary`.
5. Neu khong:
   - Chan truy cap.

Trong thiet ke hien tai:

- Ca `Admin`, `WarehouseStaff`, `Viewer` deu duoc xem.

### 8.3. Luong 3 - Admin quan ly category

1. Admin vao man `Categories`.
2. He thong hien form tao/sua category.
3. Admin nhap ten va mo ta.
4. He thong kiem tra:
   - Da dang nhap chua.
   - Co role `Admin` khong.
   - Ten category co trung khong.
5. Neu hop le:
   - Luu category.
6. Neu khong hop le:
   - Bao loi.

Neu WarehouseStaff hoac Viewer vao man nay:

- Van co the xem danh sach.
- Khong thay form tao/sua.
- Neu co gang goi API tao/sua/xoa truc tiep, server van chan.

### 8.4. Luong 4 - Admin quan ly product

1. Admin vao man `Products`.
2. He thong hien danh sach san pham.
3. Admin co quyen:
   - Tao product moi
   - Sua product
   - Xoa product
4. Khi tao/sua product, he thong kiem tra:
   - Category ton tai
   - SKU duy nhat
   - Nguoi thuc hien co role `Admin`

WarehouseStaff va Viewer:

- Chi duoc xem danh sach.
- Khong co nut `Create`, `Edit`, `Delete`.
- Neu goi API truc tiep van bi chan.

### 8.5. Luong 5 - WarehouseStaff tao phieu nhap kho

1. WarehouseStaff vao man `Receipts`.
2. He thong hien:
   - lich su phieu nhap
   - form tao phieu nhap
3. Nhan vien kho nhap:
   - Supplier
   - Note
   - Danh sach line hang
4. Moi line gom:
   - Product
   - Quantity
   - Unit Cost
5. He thong kiem tra:
   - Da dang nhap
   - Role co thuoc `Admin` hoac `WarehouseStaff`
   - Product hop le
   - So luong > 0
   - Unit cost >= 0
6. Neu hop le:
   - Tao receipt
   - Tang ton kho
   - Ghi ledger
7. Neu khong hop le:
   - Tu choi giao dich

Viewer:

- Chi duoc xem lich su phieu nhap.
- Khong duoc tao phieu nhap.

### 8.6. Luong 6 - WarehouseStaff tao phieu xuat kho

1. WarehouseStaff vao man `Issues`.
2. He thong hien:
   - lich su phieu xuat
   - form tao phieu xuat
3. Nhan vien kho chon san pham va so luong xuat.
4. He thong kiem tra:
   - Da dang nhap
   - Role phu hop
   - San pham ton tai
   - So luong xuat > 0
   - Ton kho du de xuat
5. Neu hop le:
   - Tao issue
   - Giam ton kho
   - Ghi ledger
6. Neu khong hop le:
   - Bao loi

Viewer:

- Chi duoc xem lich su.
- Khong duoc tao phieu.

## 9. Phan tach kiem soat giua UI va API

Trong he thong nay, phan quyen duoc dat o 2 tang:

### 9.1. Tang UI

Muc tieu:

- An nut, an form, an thao tac khong duoc phep.
- Lam giao dien de hieu hon doi voi nguoi dung.

Vi du:

- Viewer khong thay form tao receipt.
- WarehouseStaff khong thay nut xoa product.

Y nghia nghiep vu:

- Giam nham lan.
- Tang tra nghiem su dung.
- Giup nguoi dung chi thay nhung gi lien quan den vai tro cua minh.

### 9.2. Tang API

Muc tieu:

- Bao ve that su du lieu va nghiep vu.
- Ngan truong hop nguoi dung bo qua UI va goi API truc tiep.

Day moi la tang phan quyen bat buoc.

Nguyen tac:

- UI chi la lop huong dan.
- API moi la lop cuoi cung quyet dinh co cho thao tac hay khong.

## 10. Mapping role sang policy trong he thong

He thong da chuan hoa 3 policy chinh:

### 10.1. `ReadAccess`

Dung cho cac thao tac chi doc.

Bao gom role:

- `Admin`
- `WarehouseStaff`
- `Viewer`

Muc dich:

- Cho tat ca user da duoc cap quyen vao he thong co the theo doi ton kho.

### 10.2. `WarehouseOperations`

Dung cho cac thao tac van hanh kho.

Bao gom role:

- `Admin`
- `WarehouseStaff`

Muc dich:

- Chi nhung nguoi tham gia van hanh moi duoc tao giao dich lam thay doi ton kho.

### 10.3. `AdminOnly`

Dung cho thao tac quan tri du lieu master.

Bao gom role:

- `Admin`

Muc dich:

- Bao ve danh muc va cau hinh khoi thay doi trai phep.

## 11. Mapping nghiep vu sang man hinh hien tai

### 11.1. Login

Man hinh:

- `/login`

Y nghia nghiep vu:

- Diem vao bat buoc cho nguoi dung truoc khi su dung he thong.

### 11.2. Dashboard

Man hinh:

- `/`

Y nghia nghiep vu:

- Xem tong quan so lieu kho.

Vai tro:

- Admin
- WarehouseStaff
- Viewer

### 11.3. Categories

Man hinh:

- `/categories`

Y nghia nghiep vu:

- Quan ly nhom san pham.

Vai tro:

- Xem: tat ca role da dang nhap
- Tao/sua/xoa: Admin

### 11.4. Products

Man hinh:

- `/products`

Y nghia nghiep vu:

- Quan ly danh muc hang hoa.

Vai tro:

- Xem: tat ca role da dang nhap
- Tao/sua/xoa: Admin

### 11.5. Receipts

Man hinh:

- `/receipts`
- `/receipts/{id}`

Y nghia nghiep vu:

- Theo doi va lap phieu nhap kho.

Vai tro:

- Xem danh sach va chi tiet: tat ca role da dang nhap
- Tao phieu: Admin, WarehouseStaff

### 11.6. Issues

Man hinh:

- `/issues`
- `/issues/{id}`

Y nghia nghiep vu:

- Theo doi va lap phieu xuat kho.

Vai tro:

- Xem danh sach va chi tiet: tat ca role da dang nhap
- Tao phieu: Admin, WarehouseStaff

## 12. Mapping nghiep vu sang API hien tai

### 12.1. Nhom Auth

- `POST /api/auth/login`
- `POST /api/auth/logout`
- `GET /api/auth/me`

### 12.2. Nhom Category

- `GET /api/categories`
- `GET /api/categories/{id}`
- `POST /api/categories`
- `PUT /api/categories/{id}`
- `DELETE /api/categories/{id}`

Quyen:

- `GET`: ReadAccess
- `POST/PUT/DELETE`: AdminOnly

### 12.3. Nhom Product

- `GET /api/products`
- `GET /api/products/{id}`
- `POST /api/products`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`

Quyen:

- `GET`: ReadAccess
- `POST/PUT/DELETE`: AdminOnly

### 12.4. Nhom Receipt

- `GET /api/receipts`
- `GET /api/receipts/{id}`
- `POST /api/receipts`

Quyen:

- `GET`: ReadAccess
- `POST`: WarehouseOperations

### 12.5. Nhom Issue

- `GET /api/issues`
- `GET /api/issues/{id}`
- `POST /api/issues`

Quyen:

- `GET`: ReadAccess
- `POST`: WarehouseOperations

### 12.6. Nhom Inventory

- `GET /api/inventory/summary`

Quyen:

- `GET`: ReadAccess

## 13. Cac tinh huong nghiep vu mau

### 13.1. Tinh huong A - Viewer xem ton kho

Dieu kien:

- Nguoi dung dang nhap voi role `Viewer`

Ky vong nghiep vu:

- Duoc vao dashboard
- Duoc xem product
- Duoc xem receipt va issue history
- Khong duoc tao giao dich

Ket qua mong doi:

- UI khong hien cac form tao
- Neu goi API tao issue, server tra `403 Forbidden`

### 13.2. Tinh huong B - WarehouseStaff nhap kho

Dieu kien:

- Nguoi dung dang nhap voi role `WarehouseStaff`

Ky vong nghiep vu:

- Duoc tao receipt
- Duoc xem lich su receipt
- Khong duoc xoa product

Ket qua mong doi:

- Form tao receipt hien thi
- Nut xoa product khong hien thi
- Goi API `DELETE /api/products/{id}` se bi `403 Forbidden`

### 13.3. Tinh huong C - Admin quan ly danh muc

Dieu kien:

- Nguoi dung dang nhap voi role `Admin`

Ky vong nghiep vu:

- Toan quyen quan ly category va product
- Duoc tao issue, receipt

Ket qua mong doi:

- Co day du form va action tren UI
- Goi API tao/sua/xoa hop le neu du lieu hop le

## 14. Ma loi nghiep vu lien quan den auth

### 14.1. `401 Unauthorized`

Y nghia:

- Chua dang nhap hoac phien dang nhap khong hop le.

Nghiep vu:

- He thong chua xac dinh duoc nguoi dung la ai.

Huong xu ly:

- Yeu cau dang nhap lai.

### 14.2. `403 Forbidden`

Y nghia:

- Da dang nhap nhung khong du quyen.

Nghiep vu:

- He thong biet nguoi dung la ai, nhung role cua nguoi do khong duoc phep lam thao tac nay.

Huong xu ly:

- Khong cho thuc hien thao tac.
- Can lien he Admin neu can duoc cap quyen.

## 15. Gia tri nghiep vu ma auth mang lai cho du an

Sau khi co `Authentication` va `Authorization`, he thong khong con la mot ung dung demo mo hoan toan. No bat dau co dac diem cua mot he thong kho van hanh thuc te:

- Co phan tach trach nhiem
- Co gioi han quyen theo vai tro
- Co kha nang mo rong sang audit trail
- Co nen tang cho workflow phe duyet sau nay
- Co kha nang tich hop quan tri nguoi dung

Noi cach khac:

- Auth la lop xac dinh `ai dang thao tac`
- Authorization la lop xac dinh `nguoi do co duoc phep hay khong`
- RBAC la co che giup he thong kho van hanh dung vai, dung quyen, dung trach nhiem

## 16. Gioi han cua phien ban hien tai

Phan auth va authorization hien tai da dung cho `Wave 1`, nhung van con mot so gioi han:

1. Chua co man quan tri user va role.
2. Chua co chuc nang doi mat khau.
3. Chua co quen mat khau.
4. Chua co khoa tai khoan sau nhieu lan dang nhap sai.
5. Chua co audit trail gan giao dich voi user thuc hien.
6. Chua co claim chi tiet hon ngoai role.
7. Chua co luong phe duyet 2 buoc cho thao tac nhay cam.

## 17. Dinh huong nghiep vu tiep theo

De he thong kho tro nen ro rang va day du hon, huong nghiep vu tiep theo nen la:

### 17.1. Gan nguoi thuc hien vao giao dich

Moi receipt va issue nen biet:

- ai tao
- tao luc nao

Muc dich:

- truy vet trach nhiem
- doi soat nghiep vu

### 17.2. Bo sung audit trail

He thong can luu:

- ai sua product
- ai xoa category
- ai tao issue

Muc dich:

- minh bach
- truy vet
- ho tro kiem soat noi bo

### 17.3. Tach role sau hon

Ve sau co the mo rong thanh:

- `InventoryManager`
- `Procurement`
- `Auditor`
- `FinanceViewer`

## 18. Ket luan

Neu nhin du an theo luong nghiep vu, phan `Authentication` va `Authorization` la cua vao de hieu toan bo he thong:

- Truoc het phai biet ai dang su dung he thong.
- Sau do phai biet ho duoc phep lam gi.
- Tu day moi hieu vi sao co man hinh chi xem, co man hinh duoc thao tac, va vi sao mot so thao tac bi chan.

Tom gon lai:

- `Admin` quan tri va kiem soat du lieu master.
- `WarehouseStaff` van hanh nghiep vu nhap xuat kho.
- `Viewer` theo doi, xem bao cao, khong thao tac.

Day la nen tang dung cho mot he thong quan ly kho ro trach nhiem, ro quyen han, va de mo rong sau nay.
