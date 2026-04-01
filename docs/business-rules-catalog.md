# Business Rules Catalog - Danh Muc Toan Bo Quy Tac Nghiep Vu

## 1. Muc dich tai lieu

Tai lieu nay gom toan bo business rules cua he thong thanh mot noi de tra cuu.

Muc tieu:

- giup ban thay ro he thong dang tuan theo nhung quy tac nao
- tach business rules ra khoi source code va UI
- de dung cho:
  - phan tich nghiep vu
  - nghiem thu
  - review thay doi
  - dao tao nhan vien

## 2. Cach doc catalog

Moi rule duoc viet theo:

- `Rule ID`
- `Ten rule`
- `Mo ta`
- `Pham vi`
- `Loai rule`
- `Ly do business`
- `Tac dong neu vi pham`

Loai rule co the la:

- `Validation`
- `Permission`
- `Calculation`
- `Integrity`
- `Workflow`

## 3. Nhom quy tac authentication va authorization

### BR-AUTH-001 - User phai dang nhap de vao he thong

- Pham vi: toan he thong nghiep vu
- Loai rule: Permission
- Mo ta: Moi man hinh nghiep vu chinh yeu cau user phai duoc xac thuc.
- Ly do business: He thong kho phai biet ai dang thao tac.
- Tac dong neu vi pham: Mat kiem soat, khong truy vet duoc actor.

### BR-AUTH-002 - Chi user da dang nhap moi duoc goi API doc du lieu

- Pham vi: categories, products, receipts, issues, inventory summary
- Loai rule: Permission
- Mo ta: Cac API chi doc thuoc `ReadAccess`.
- Ly do business: Khong mo du lieu kho cho nguoi ngoai he thong.

### BR-AUTH-003 - Chi Admin duoc quan ly master data

- Pham vi: categories, products
- Loai rule: Permission
- Mo ta: Tao, sua, xoa category/product chi danh cho Admin.
- Ly do business: Master data phai duoc kiem soat tap trung.

### BR-AUTH-004 - Chi Admin va WarehouseStaff duoc tao giao dich kho

- Pham vi: receipts, issues
- Loai rule: Permission
- Mo ta: Viewer khong duoc tao giao dich lam bien dong ton kho.
- Ly do business: Tach ro nguoi van hanh va nguoi chi xem.

## 4. Nhom quy tac Category

### BR-CAT-001 - Category name la bat buoc

- Pham vi: create/update category
- Loai rule: Validation
- Ly do business: Category khong the vo danh.

### BR-CAT-002 - Category name toi da 100 ky tu

- Pham vi: create/update category
- Loai rule: Validation
- Ly do business: Chuan hoa du lieu va tranh ten qua dai.

### BR-CAT-003 - Category description toi da 300 ky tu

- Pham vi: create/update category
- Loai rule: Validation
- Ly do business: Giu thong tin mo ta gon va nhat quan.

### BR-CAT-004 - Category name phai duy nhat

- Pham vi: create/update category
- Loai rule: Integrity
- Ly do business: Tranh duplicate nhom san pham.
- Tac dong neu vi pham: Bao cao theo nhom bi trung lap va kho dung.

### BR-CAT-005 - Khong duoc xoa category neu van con product

- Pham vi: delete category
- Loai rule: Integrity
- Ly do business: Product phai thuoc category hop le.
- Tac dong neu vi pham: Product mat ngu canh danh muc.

## 5. Nhom quy tac Product

### BR-PROD-001 - SKU la bat buoc

- Pham vi: create/update product
- Loai rule: Validation
- Ly do business: Moi product phai co ma nhan dien.

### BR-PROD-002 - SKU phai duy nhat

- Pham vi: create/update product
- Loai rule: Integrity
- Ly do business: SKU la khoa nghiep vu cua product.

### BR-PROD-003 - Product name la bat buoc

- Pham vi: create/update product
- Loai rule: Validation
- Ly do business: Product can co ten de van hanh.

### BR-PROD-004 - CategoryId phai ton tai

- Pham vi: create/update product
- Loai rule: Integrity
- Ly do business: Product phai thuoc category hop le.

### BR-PROD-005 - ReorderLevel khong duoc am

- Pham vi: create/update product
- Loai rule: Validation
- Ly do business: Nguong canh bao khong the am.

### BR-PROD-006 - Product moi tao co ton bang 0

- Pham vi: create product
- Loai rule: Workflow
- Ly do business: Product moi chi la master data, chua co hang thuc te vao kho.

### BR-PROD-007 - Product moi tao co average cost bang 0

- Pham vi: create product
- Loai rule: Calculation
- Ly do business: Chua co lo nhap nao de hinh thanh gia von.

### BR-PROD-008 - OnHandQty khong duoc am

- Pham vi: toan he thong
- Loai rule: Integrity
- Ly do business: He thong kho khong duoc cho phep ton am.

### BR-PROD-009 - Product inactive khong duoc tham gia giao dich moi

- Pham vi: receipt, issue
- Loai rule: Workflow
- Ly do business: Product da ngung van hanh khong nen dua vao phieu moi.

### BR-PROD-010 - Khong duoc xoa product neu da co lich su giao dich

- Pham vi: delete product
- Loai rule: Integrity
- Ly do business: Giu toan ven lich su nghiep vu.

## 6. Nhom quy tac Receipt

### BR-RCPT-001 - Receipt phai co it nhat 1 line

- Pham vi: create receipt
- Loai rule: Validation
- Ly do business: Phieu nhap khong co dong hang la phieu vo nghia.

### BR-RCPT-002 - Product trong receipt phai ton tai va active

- Pham vi: create receipt
- Loai rule: Integrity
- Ly do business: Khong duoc nhap cho mat hang khong hop le.

### BR-RCPT-003 - Receipt quantity phai > 0

- Pham vi: create receipt
- Loai rule: Validation
- Ly do business: Nhap kho phai lam tang ton.

### BR-RCPT-004 - Receipt unit cost khong duoc am

- Pham vi: create receipt
- Loai rule: Validation
- Ly do business: Gia nhap am khong hop le nghiep vu.

### BR-RCPT-005 - LineTotal cua receipt = Quantity * UnitCost

- Pham vi: create receipt
- Loai rule: Calculation
- Ly do business: Tong gia tri dong hang duoc tinh tu so luong va gia nhap.

### BR-RCPT-006 - TotalAmount cua receipt = tong line total

- Pham vi: create receipt
- Loai rule: Calculation
- Ly do business: Tong phieu nhap phai bang tong cac dong.

### BR-RCPT-007 - Receipt phai cap nhat on hand qty

- Pham vi: create receipt
- Loai rule: Workflow
- Ly do business: Nhap kho lam tang ton.

### BR-RCPT-008 - Receipt phai tinh lai average cost

- Pham vi: create receipt
- Loai rule: Calculation
- Ly do business: Lo nhap moi co the thay doi gia von binh quan.

### BR-RCPT-009 - Receipt phai tao ledger entry

- Pham vi: create receipt
- Loai rule: Workflow
- Ly do business: Can truy vet bien dong kho.

### BR-RCPT-010 - Receipt phai duoc xu ly atomically

- Pham vi: create receipt
- Loai rule: Integrity
- Ly do business: Khong duoc xay ra cap nhat nua chung.

## 7. Nhom quy tac Issue

### BR-ISS-001 - Issue phai co it nhat 1 line

- Pham vi: create issue
- Loai rule: Validation
- Ly do business: Phieu xuat phai co noi dung xuat.

### BR-ISS-002 - Product trong issue phai ton tai va active

- Pham vi: create issue
- Loai rule: Integrity
- Ly do business: Khong duoc xuat cho mat hang khong hop le.

### BR-ISS-003 - Issue quantity phai > 0

- Pham vi: create issue
- Loai rule: Validation
- Ly do business: Xuat kho phai la so duong.

### BR-ISS-004 - Khong duoc xuat vuot ton

- Pham vi: create issue
- Loai rule: Integrity
- Ly do business: Ton kho khong du thi khong duoc xuat.

### BR-ISS-005 - UnitCost cua issue = AverageCost hien tai

- Pham vi: create issue
- Loai rule: Calculation
- Ly do business: He thong dang xuat theo weighted average cost.

### BR-ISS-006 - LineTotal cua issue = Quantity * AverageCost

- Pham vi: create issue
- Loai rule: Calculation
- Ly do business: Gia tri xuat duoc tinh theo gia von hien tai.

### BR-ISS-007 - TotalAmount cua issue = tong line total

- Pham vi: create issue
- Loai rule: Calculation
- Ly do business: Tong gia tri phieu xuat phai bang tong cac dong.

### BR-ISS-008 - Issue phai giam on hand qty

- Pham vi: create issue
- Loai rule: Workflow
- Ly do business: Xuat kho lam giam ton.

### BR-ISS-009 - Issue khong tinh lai average cost

- Pham vi: create issue
- Loai rule: Calculation
- Ly do business: Weighted average chi doi khi nhap, khong doi khi xuat.

### BR-ISS-010 - Issue phai tao ledger entry

- Pham vi: create issue
- Loai rule: Workflow
- Ly do business: Can truy vet bien dong ton kho.

### BR-ISS-011 - Issue phai duoc xu ly atomically

- Pham vi: create issue
- Loai rule: Integrity
- Ly do business: Tranh tinh trang luu nua chung.

## 8. Nhom quy tac Inventory Summary

### BR-SUM-001 - Summary chi tinh tren product active

- Pham vi: inventory summary
- Loai rule: Calculation
- Ly do business: Chi san pham dang van hanh moi thuoc pham vi dashboard hien tai.

### BR-SUM-002 - TotalProducts = so product active

- Pham vi: inventory summary
- Loai rule: Calculation
- Ly do business: Can biet quy mo danh muc dang van hanh.

### BR-SUM-003 - TotalOnHandUnits = tong OnHandQty

- Pham vi: inventory summary
- Loai rule: Calculation
- Ly do business: Cho biet tong luong hang dang co.

### BR-SUM-004 - TotalInventoryValue = tong (OnHandQty * AverageCost)

- Pham vi: inventory summary
- Loai rule: Calculation
- Ly do business: Cho biet tong gia tri ton kho.

### BR-SUM-005 - Low stock khi OnHandQty <= ReorderLevel

- Pham vi: inventory summary
- Loai rule: Calculation
- Ly do business: Day la nguong canh bao sap het hang.

### BR-SUM-006 - Low stock items sap xep theo ton tang dan, roi theo ten

- Pham vi: inventory summary
- Loai rule: Workflow
- Ly do business: Uu tien nhin mat hang thieu nghiem trong hon truoc.

## 9. Nhom quy tac Ledger

### BR-LEDGER-001 - Moi receipt line phai tao 1 ledger entry

- Pham vi: receipt processing
- Loai rule: Workflow
- Ly do business: Moi bien dong nhap can dau vet.

### BR-LEDGER-002 - Moi issue line phai tao 1 ledger entry

- Pham vi: issue processing
- Loai rule: Workflow
- Ly do business: Moi bien dong xuat can dau vet.

### BR-LEDGER-003 - Ledger phai luu running on hand qty

- Pham vi: ledger
- Loai rule: Integrity
- Ly do business: De biet sau giao dich ton con bao nhieu.

### BR-LEDGER-004 - Ledger phai luu running average cost

- Pham vi: ledger
- Loai rule: Integrity
- Ly do business: De truy vet gia von sau moi bien dong.

### BR-LEDGER-005 - MovementType phai phan biet receipt va issue

- Pham vi: ledger
- Loai rule: Validation
- Ly do business: De hieu ly do bien dong.

## 10. Nhom quy tac giao dien va phan quyen hien thi

### BR-UI-001 - User khong du quyen thi khong hien form thao tac

- Pham vi: UI
- Loai rule: Permission
- Ly do business: Giam nham lan va giam rui ro thao tac sai.

### BR-UI-002 - Viewer duoc xem nhung khong duoc tao giao dich

- Pham vi: UI + API
- Loai rule: Permission
- Ly do business: Viewer la role phuc vu theo doi, khong phai van hanh.

### BR-UI-003 - WarehouseStaff khong duoc thay action quan tri master data

- Pham vi: UI + API
- Loai rule: Permission
- Ly do business: Tach van hanh kho voi quan tri danh muc.

## 11. Nhom quy tac roadmap mo rong

### BR-FUT-001 - He thong can co stock adjustment de xu ly lech ton

- Pham vi: roadmap
- Loai rule: Workflow
- Ly do business: Kho thuc te luon co chenh lech kiem ke.

### BR-FUT-002 - He thong nen co supplier/customer master

- Pham vi: roadmap
- Loai rule: Integrity
- Ly do business: Tranh text rac va ho tro thong ke theo doi tac.

### BR-FUT-003 - He thong nen gan user vao giao dich

- Pham vi: roadmap
- Loai rule: Integrity
- Ly do business: Truy vet ai tao giao dich.

### BR-FUT-004 - He thong nen co audit trail

- Pham vi: roadmap
- Loai rule: Integrity
- Ly do business: Theo doi ai sua doi du lieu.

## 12. Cach dung catalog nay de hoc business

Ban co the dung tai lieu nay theo 3 cach:

### Cach 1 - Hoc theo nhom rule

Vi du:

- hoc toan bo rule ve product
- hoc toan bo rule ve receipt

### Cach 2 - Hoc theo loai rule

Vi du:

- tim toan bo rule `Calculation`
- tim toan bo rule `Permission`

Day la cach rat tot de hieu:

- phan nao la cong thuc business
- phan nao la control

### Cach 3 - Dung khi review thay doi

Khi co mot feature moi, hay hoi:

- no anh huong rule nao
- no them rule nao moi
- no lam vo rule nao cu

## 13. Ket luan

Business rules la "xuong song" cua he thong nghiep vu.

Code co the thay doi.
UI co the thay doi.
API co the doi ten.

Nhung neu business rules khong ro, he thong se mat tinh nhat quan.

Tai lieu nay giup ban nhin he thong o muc:

- quy tac nao dang dieu khien no
- tai sao quy tac do ton tai
- va business se vo o dau neu quy tac do bi pha
