# Event Storming Notes - Phan Tich He Thong Theo Actor, Command, Event, Policy

## 1. Muc dich tai lieu

Tai lieu nay dung ky thuat tu duy `Event Storming` de phan tich he thong kho.

Muc tieu:

- giup ban nhin he thong theo dong chay su kien nghiep vu
- thay ro actor nao phat lenh nao
- thay he thong phan ung bang event nao
- thay rule, policy, checkpoint nam o dau

Day la cach hoc business rat manh vi no buoc ban nhin:

- ai hanh dong
- hanh dong gi
- he thong thay doi gi
- quy tac nao chen vao giua

Neu glossary giup ban hieu "tu vung",  
neu use case giup ban hieu "luong nghiep vu",  
thi event storming giup ban hieu "dong luc van hanh" cua he thong.

## 2. Event storming la gi

`Event Storming` la cach phan tich domain bang cac thanh phan chinh:

- `Actor`: ai thuc hien
- `Command`: nguoi do yeu cau he thong lam gi
- `Policy`: quy tac nao kiem soat
- `Event`: dieu gi da xay ra trong nghiep vu
- `Read model / View`: thong tin nao duoc hien ra de nguoi dung quan sat

Noi ngan gon:

- actor bam lenh
- policy kiem tra
- event xay ra
- he thong cap nhat state va hien ra ket qua

## 3. Cach doc tai lieu nay

Moi dong nghiep vu se duoc viet theo mau:

`Actor -> Command -> Policy -> Event -> State change -> Read model`

Day la format cuc tot de hoc business vi no ep ban tra loi day du:

- ai
- muon lam gi
- bi rang buoc boi gi
- ket qua business la gi
- nguoi dung nhin thay gi

## 4. Bounded context hien tai cua du an

He thong hien tai co the tam chia thanh 4 context nghiep vu:

1. `Identity and Access`
2. `Catalog Management`
3. `Inventory Transactions`
4. `Inventory Visibility and Traceability`

### 4.1. Identity and Access

Noi xay ra:

- login
- logout
- xac dinh role
- phan quyen action

### 4.2. Catalog Management

Noi xay ra:

- tao/sua/xoa category
- tao/sua/xoa product

### 4.3. Inventory Transactions

Noi xay ra:

- tao receipt
- tao issue
- cap nhat on hand
- cap nhat average cost

### 4.4. Inventory Visibility and Traceability

Noi xay ra:

- inventory summary
- low stock list
- receipt history
- issue history
- ledger

## 5. Event storming cho context Identity and Access

### 5.1. Luong dang nhap

#### Actor

- Admin
- WarehouseStaff
- Viewer

#### Command

- `LoginRequested`

#### Policy

1. User phai ton tai
2. Password phai dung

#### Event

- `UserAuthenticated`

#### State change

- session duoc tao
- user duoc gan voi role

#### Read model

- login success
- menu hien theo role
- nguoi dung duoc vao he thong

### 5.2. Luong dang nhap that bai

#### Actor

- user bat ky

#### Command

- `LoginRequested`

#### Policy

- credentials khong hop le

#### Event

- `AuthenticationRejected`

#### State change

- khong tao session

#### Read model

- thong bao sai username/email hoac password

### 5.3. Luong dang xuat

#### Actor

- user da dang nhap

#### Command

- `LogoutRequested`

#### Policy

- session hien tai ton tai

#### Event

- `UserSignedOut`

#### State change

- session bi huy

#### Read model

- quay ve login

## 6. Event storming cho context Catalog Management

### 6.1. Tao category

#### Actor

- Admin

#### Command

- `CreateCategory`

#### Policy

1. Actor phai la Admin
2. Category name khong duoc rong
3. Category name phai unique

#### Event

- `CategoryCreated`

#### State change

- them 1 category moi vao catalog

#### Read model

- category xuat hien trong list
- co the dung khi tao product

### 6.2. Sua category

#### Actor

- Admin

#### Command

- `UpdateCategory`

#### Policy

1. Actor phai la Admin
2. Category ton tai
3. Ten moi phai unique

#### Event

- `CategoryUpdated`

#### State change

- category doi ten/mo ta

#### Read model

- list category cap nhat thong tin moi

### 6.3. Xoa category

#### Actor

- Admin

#### Command

- `DeleteCategory`

#### Policy

1. Actor phai la Admin
2. Category ton tai
3. Category khong duoc co product nao tham chieu

#### Event

- `CategoryDeleted`

#### State change

- category bi xoa khoi catalog

#### Read model

- category bien mat khoi list

### 6.4. Tao product

#### Actor

- Admin

#### Command

- `CreateProduct`

#### Policy

1. Actor phai la Admin
2. SKU phai unique
3. Category phai ton tai
4. ReorderLevel >= 0

#### Event

- `ProductCreated`

#### State change

- them product moi vao catalog
- state ban dau:
  - on hand = 0
  - average cost = 0
  - active = true

#### Read model

- product xuat hien trong danh sach

### 6.5. Sua product

#### Actor

- Admin

#### Command

- `UpdateProduct`

#### Policy

1. Actor phai la Admin
2. Product ton tai
3. SKU phai unique
4. Category phai ton tai

#### Event

- `ProductUpdated`

#### State change

- product doi thong tin nghiep vu

#### Read model

- list product cap nhat

### 6.6. Xoa product

#### Actor

- Admin

#### Command

- `DeleteProduct`

#### Policy

1. Actor phai la Admin
2. Product ton tai
3. Product khong duoc co receipt line
4. Product khong duoc co issue line

#### Event

- `ProductDeleted`

#### State change

- product bi xoa khoi catalog

#### Read model

- product bien mat khoi list

## 7. Event storming cho context Inventory Transactions - Receipt

### 7.1. Tao receipt thanh cong

#### Actor

- Admin
- WarehouseStaff

#### Command

- `CreateReceipt`

#### Policy

1. Actor phai co quyen warehouse operation
2. Receipt phai co it nhat 1 line
3. Moi line phai co product ton tai va active
4. Quantity > 0
5. UnitCost >= 0

#### Event chain

1. `ReceiptCreationStarted`
2. `ReceiptLineAccepted`
3. `ProductInventoryIncreased`
4. `ProductAverageCostRecalculated`
5. `InventoryLedgerRecordedForReceipt`
6. `ReceiptCreated`

#### State change

Voi moi line:

- on hand tang
- average cost co the doi
- ledger them dong moi

Voi toan bo phieu:

- them 1 receipt moi vao history

#### Read model

- receipt history co phieu moi
- dashboard cap nhat ton moi
- product list cap nhat on hand va average cost

### 7.2. Tao receipt that bai do product inactive

#### Actor

- Admin
- WarehouseStaff

#### Command

- `CreateReceipt`

#### Policy

- product phai active

#### Event

- `ReceiptRejected`

#### State change

- khong co thay doi ton
- khong co receipt moi
- khong co ledger moi

#### Read model

- hien thong bao loi product khong ton tai hoac inactive

### 7.3. Tao receipt that bai do line khong hop le

#### Command

- `CreateReceipt`

#### Policy

- quantity > 0
- unit cost khong am

#### Event

- `ReceiptValidationFailed`

#### State change

- khong thay doi du lieu

## 8. Event storming cho context Inventory Transactions - Issue

### 8.1. Tao issue thanh cong

#### Actor

- Admin
- WarehouseStaff

#### Command

- `CreateIssue`

#### Policy

1. Actor phai co quyen warehouse operation
2. Issue phai co it nhat 1 line
3. Product phai ton tai va active
4. Quantity > 0
5. Ton kho phai du

#### Event chain

1. `IssueCreationStarted`
2. `IssueLineAccepted`
3. `InventoryAvailabilityConfirmed`
4. `ProductInventoryDecreased`
5. `InventoryLedgerRecordedForIssue`
6. `IssueCreated`

#### State change

Voi moi line:

- on hand giam
- average cost giu nguyen
- ledger them dong moi

Voi toan bo phieu:

- them 1 issue moi vao history

#### Read model

- issue history co phieu moi
- dashboard cap nhat ton moi
- product list cap nhat on hand

### 8.2. Tao issue that bai do khong du ton

#### Actor

- Admin
- WarehouseStaff

#### Command

- `CreateIssue`

#### Policy

- OnHandQty phai >= quantity yeu cau

#### Event

- `IssueRejectedForInsufficientStock`

#### State change

- khong co thay doi ton
- khong co phieu issue moi

#### Read model

- hien thong bao san pham nao khong du ton

### 8.3. Tao issue that bai do product inactive

#### Command

- `CreateIssue`

#### Policy

- product phai active

#### Event

- `IssueRejected`

#### State change

- khong doi du lieu

## 9. Event storming cho context Visibility and Traceability

### 9.1. Xem inventory summary

#### Actor

- bat ky user da dang nhap nao co read access

#### Command

- `LoadInventorySummary`

#### Policy

- actor phai duoc phep xem

#### Event

- `InventorySummaryCalculated`

#### State change

- khong doi state luu tru
- day la read operation

#### Read model

- total products
- total on hand units
- total inventory value
- low stock count
- low stock items

### 9.2. Xem receipt history

#### Actor

- user co read access

#### Command

- `LoadReceiptHistory`

#### Event

- `ReceiptHistoryViewed`

#### State change

- khong doi state nghiep vu

### 9.3. Xem issue history

#### Actor

- user co read access

#### Command

- `LoadIssueHistory`

#### Event

- `IssueHistoryViewed`

#### State change

- khong doi state nghiep vu

## 10. Event storming cho truong hop role bi chan

### 10.1. Viewer co gang tao issue

#### Actor

- Viewer

#### Command

- `CreateIssue`

#### Policy

- chi Admin va WarehouseStaff moi duoc warehouse operations

#### Event

- `PermissionDenied`

#### State change

- khong doi gi trong inventory

#### Read model

- UI an form issue create
- neu goi truc tiep API thi tra `403`

### 10.2. WarehouseStaff co gang xoa product

#### Actor

- WarehouseStaff

#### Command

- `DeleteProduct`

#### Policy

- chi Admin duoc quan ly master data

#### Event

- `PermissionDenied`

#### State change

- product khong bi xoa

## 11. Policy map tong hop

Co the nhin he thong qua 3 policy lon:

### Policy A - ReadAccess

Cho phep:

- xem categories
- xem products
- xem receipts
- xem issues
- xem inventory summary

### Policy B - WarehouseOperations

Cho phep:

- tao receipt
- tao issue

### Policy C - AdminOnly

Cho phep:

- tao/sua/xoa category
- tao/sua/xoa product

## 12. Event nao la event business that su

Khi hoc event storming, can biet event nao co gia tri business that su:

### Event business core

- `CategoryCreated`
- `ProductCreated`
- `ReceiptCreated`
- `IssueCreated`
- `ProductInventoryIncreased`
- `ProductInventoryDecreased`
- `ProductAverageCostRecalculated`
- `PermissionDenied`
- `IssueRejectedForInsufficientStock`

### Event ky thuat ho tro

- `LoadReceiptHistory`
- `LoadIssueHistory`
- `InventorySummaryCalculated`

Day la distinction quan trong:

- co event lam doi nghiep vu
- co event chi phuc vu hien thi/tra cuu

## 13. Command nao dang lam thay doi state

Trong du an nay, cac command thay doi state business la:

- `CreateCategory`
- `UpdateCategory`
- `DeleteCategory`
- `CreateProduct`
- `UpdateProduct`
- `DeleteProduct`
- `CreateReceipt`
- `CreateIssue`

Cac command chi doc:

- `LoadInventorySummary`
- `LoadReceiptHistory`
- `LoadIssueHistory`
- `LoadProductList`
- `LoadCategoryList`

Phan biet command thay doi state va command doc la co so cua:

- CQRS
- audit
- idempotency
- optimization sau nay

## 14. Diem bat dau cua cac future event

Khi he thong mo rong, mot so event business moi se xuat hien:

### Khi co stock adjustment

- `StockAdjusted`
- `InventoryCorrectionApplied`

### Khi co audit trail

- `EntityChangeRecorded`

### Khi co supplier master

- `SupplierCreated`
- `SupplierAssignedToReceipt`

### Khi co reorder workflow

- `LowStockDetected`
- `ReorderRecommendationGenerated`

### Khi co approval

- `IssueSubmittedForApproval`
- `IssueApproved`
- `IssueRejected`

## 15. Cach tu lam event storming tren mot feature moi

Ban co the dung cong thuc sau:

1. Ai muon lam gi?
2. Lenh do ten la gi?
3. Quy tac nao phai kiem tra truoc?
4. Neu hop le thi su kien business nao xay ra?
5. State nao doi?
6. Nguoi dung se thay gi tren UI?

Vi du voi `StockAdjustment`:

- Actor: WarehouseStaff
- Command: AdjustStock
- Policy: co quyen, co ly do, khong duoc am ton neu rule khong cho
- Event: StockAdjusted
- State change: OnHandQty doi
- Read model: stock card cap nhat

## 16. Bai hoc business quan trong tu event storming

Neu hoc ky, event storming day ban 4 bai hoc lon:

### Bai hoc 1

He thong business khong phai la tap hop man hinh.  
No la tap hop phan ung voi command cua actor.

### Bai hoc 2

Moi thay doi state co mot event business dung sau.

### Bai hoc 3

Policy la noi business rule chen vao giua y dinh va ket qua.

### Bai hoc 4

Read model la "anh chup" de actor ra quyet dinh, khong phai toan bo su that nghiep vu.

## 17. Ket luan

Event storming giup ban nhin du an nay rat ro:

- ai bam lenh
- lenh gi duoc phep
- event nao xay ra
- ton kho doi nhu the nao
- du lieu nao hien ra sau do

Neu use case la cach ke cau chuyen he thong theo tung quy trinh,  
thi event storming la cach nhin he thong nhu mot dong chay su kien business co logic va quy tac rat ro.
