# Domain Model Explained - Noi Business Voi Domain Model Trong Code

## 1. Muc dich tai lieu

Tai lieu nay giai thich cach business duoc anh xa thanh `domain model` trong code.

Day la tai lieu cau noi giua:

- nguoi hoc business
- nguoi doc code

Muc tieu:

- giup ban hieu vi sao entity ton tai
- moi entity dai dien cho y nghia nghiep vu nao
- relationship giua cac entity phan anh quy trinh business ra sao
- DTO va Request khac entity nhu the nao

Neu ban muon doc code ma khong bi "mat business", tai lieu nay la phan quan trong nhat.

## 2. Domain model la gi

`Domain model` la cach he thong bieu dien the gioi nghiep vu bang cac object.

No khong phai chi la table database.

No la tap hop cac khai niem nghiep vu duoc dat thanh:

- entity
- field
- relationship
- rule

Trong du an nay, domain model nam chu yeu o:

- `Shared/Domain`

## 3. Nhom doi tuong trong domain

Co the chia domain hien tai thanh 3 nhom:

### 3.1. Master entities

- `Category`
- `Product`

### 3.2. Transaction entities

- `StockReceipt`
- `StockReceiptLine`
- `StockIssue`
- `StockIssueLine`

### 3.3. History entity

- `InventoryLedgerEntry`

Nhom phu tro:

- `ApplicationUser` o tang auth

## 4. Category - Domain cua nhom san pham

### Y nghia business

`Category` dai dien cho nhom phan loai san pham.

### Tai sao can entity rieng

Neu category chi la text nam trong product:

- se de trung lap
- de sai chinh ta
- kho bao cao theo nhom

Nên he thong tach thanh entity rieng.

### Thuoc tinh chinh

- `Id`
- `Name`
- `Description`
- `CreatedAtUtc`
- `Products`

### Cach hieu quan he

`Category.Products` cho thay:

- mot category co the chua nhieu product

Ve business, day la quan he:

- one-to-many

### Business rule an sau model nay

- category la noi "gom nhom"
- product khong duoc ton tai trong he thong ma khong thuoc category hop le

## 5. Product - Domain trung tam cua ton kho

### Y nghia business

`Product` la mat hang cu the ma kho dang quan ly.

### Tai sao product la entity trung tam

Tat ca nghiep vu kho deu xoay quanh product:

- nhap cai gi
- xuat cai gi
- ton cua cai gi
- gia tri cua cai gi

### Thuoc tinh chinh

- `Sku`
- `Name`
- `Description`
- `CategoryId`
- `OnHandQty`
- `AverageCost`
- `ReorderLevel`
- `IsActive`
- `CreatedAtUtc`
- `LastUpdatedUtc`

### Cach doc product theo business

- `Sku`: ma nghiep vu
- `Name`: ten mat hang
- `CategoryId`: thuoc nhom nao
- `OnHandQty`: con bao nhieu
- `AverageCost`: gia von hien tai
- `ReorderLevel`: nguong canh bao
- `IsActive`: con duoc giao dich hay khong

### Tai sao `OnHandQty` va `AverageCost` nam ngay tren Product

Vi day la `current state`.

Neu nguoi dung hoi:

- "Wireless Mouse con bao nhieu?"

He thong khong nen phai tinh lai tu toan bo lich su moi lan.  
No can co state hien tai luu san.

### Quan he cua Product

- `Category`
- `ReceiptLines`
- `IssueLines`
- `LedgerEntries`

Quan he nay noi len:

- product duoc dung trong nhieu giao dich
- product co lich su bien dong rieng

## 6. StockReceipt - Domain cua giao dich nhap kho

### Y nghia business

`StockReceipt` la dau phieu nhap kho.

### Tai sao can entity dau phieu

Vi mot lan nhap kho khong chi la 1 line.  
No thuong co:

- supplier
- note
- thoi gian
- tong gia tri
- nhieu dong hang

Nen can mot entity "header".

### Thuoc tinh chinh

- `DocumentNo`
- `ReceivedAtUtc`
- `Supplier`
- `Note`
- `TotalAmount`
- `Lines`

### Tai sao `TotalAmount` nam tren header

Vi trong business, nguoi dung can:

- tong gia tri phieu

ma khong can cong line bang tay moi lan.

### Y nghia quan he `Lines`

1 receipt co nhieu dong nhap.

Do la cau truc header-detail rat pho bien trong he thong nghiep vu.

## 7. StockReceiptLine - Domain cua tung dong nhap kho

### Y nghia business

`StockReceiptLine` la mot dong chi tiet trong phieu nhap.

### Thuoc tinh chinh

- `StockReceiptId`
- `ProductId`
- `Quantity`
- `UnitCost`
- `LineTotal`

### Tai sao line phai tach rieng

Vi 1 receipt co the nhap:

- nhieu product
- moi product so luong khac nhau
- gia khac nhau

### Business meaning cua tung field

- `ProductId`: hang nao duoc nhap
- `Quantity`: nhap bao nhieu
- `UnitCost`: gia nhap tren moi don vi
- `LineTotal`: tong gia tri cua dong do

## 8. StockIssue - Domain cua giao dich xuat kho

### Y nghia business

`StockIssue` la dau phieu xuat kho.

### Tai sao cau truc giong receipt

Vi ve nghiep vu, xuat kho cung la mot chung tu header-detail:

- co dau phieu
- co thong tin nguoi nhan
- co ngay gio
- co tong gia tri
- co nhieu line

### Thuoc tinh chinh

- `DocumentNo`
- `IssuedAtUtc`
- `Customer`
- `Note`
- `TotalAmount`
- `Lines`

### Diem khac business so voi receipt

- receipt la dong vao
- issue la dong ra
- receipt co unit cost do user nhap
- issue co unit cost lay tu average cost hien tai

## 9. StockIssueLine - Domain cua tung dong xuat kho

### Y nghia business

`StockIssueLine` la mot dong chi tiet xuat kho.

### Thuoc tinh chinh

- `StockIssueId`
- `ProductId`
- `Quantity`
- `UnitCost`
- `LineTotal`

### Business meaning

- hang nao da xuat
- xuat bao nhieu
- xuat theo gia von nao
- tong gia tri xuat cua dong do la bao nhieu

## 10. InventoryLedgerEntry - Domain cua lich su bien dong

### Y nghia business

Day la entity "lich su truy vet".

Neu `Product` la state hien tai, thi `InventoryLedgerEntry` la dau vet cua nhung su kien da tao ra state do.

### Thuoc tinh chinh

- `ProductId`
- `MovementType`
- `ReferenceNo`
- `OccurredAtUtc`
- `QuantityChange`
- `UnitCost`
- `ValueChange`
- `RunningOnHandQty`
- `RunningAverageCost`

### Tai sao entity nay rat quan trong

No cho phep he thong:

- explain duoc state hien tai
- truy vet duoc receipt/issue nao gay bien dong
- ho tro stock card sau nay

### Tai sao can `RunningOnHandQty` va `RunningAverageCost`

Vi neu chi luu `QuantityChange`, ve sau muon biet sau giao dich do ton con bao nhieu se phai tinh lai toan bo tu dau.

Luu running value la cach rat thuc dung de:

- truy vet nhanh
- report nhanh

## 11. ApplicationUser - Domain cua nguoi su dung he thong

### Y nghia business

`ApplicationUser` la con nguoi duoc cap quyen truy cap he thong.

### Trong du an nay

Entity nay hien tai phuc vu:

- dang nhap
- role
- hien ten user

### Gia tri business

No la nen tang de ve sau gan nguoi thao tac vao:

- receipt
- issue
- audit trail

## 12. Entity va DTO khac nhau nhu the nao

Day la diem cuc ky quan trong de hoc code tu goc business.

### 12.1. Entity

Entity la object noi bo he thong dung de:

- luu database
- bieu dien domain
- quan ly quan he giua du lieu

Vi du:

- `Product`
- `StockReceipt`

### 12.2. DTO

DTO la object dung de:

- gui du lieu ra UI
- nhan du lieu tu client
- gioi han thong tin trao doi

Vi du:

- `ProductDto`
- `CreateProductRequest`
- `StockReceiptDetailDto`

### 12.3. Tai sao khong tra thang entity ra UI

Vi entity chua:

- du lieu noi bo
- navigation property
- cau truc kho truyen qua API

DTO giup:

- ro nghiep vu can hien gi
- tranh lo du lieu khong can
- de tien hoa API

## 13. Request model la gi trong business

`Request` model la mo ta "toi muon he thong thuc hien dieu gi".

Vi du:

- `CreateProductRequest`: toi muon tao mot product moi
- `CreateStockReceiptRequest`: toi muon nhap kho mot lo hang moi

Day khong phai la state cua he thong.  
Day la y dinh nghiep vu cua nguoi dung.

Can tach ro:

- `Request`: y dinh
- `Entity`: du lieu thuc te cua he thong
- `Dto`: thong tin duoc trinh bay/tra ve

## 14. Vi sao InventoryService la noi business phat huy ro nhat

Trong code, `InventoryService` la noi business rule duoc the hien dam net nhat.

Tai day he thong:

- validate receipt/issue
- cap nhat on hand qty
- cap nhat average cost
- tao ledger
- wrap trong database transaction

Noi cach khac:

- controller la diem vao
- entity la mo hinh du lieu
- service la noi nghiep vu "chay"

## 15. Database transaction va business atomicity

Khi tao receipt hoac issue, he thong mo database transaction.

### Tai sao business can dieu nay

Vi mot giao dich kho thuc te phai thanh cong tron ven:

- tao dau phieu
- tao cac line
- cap nhat product
- tao ledger

Khong duoc phep xay ra truong hop:

- da tang ton nhung chua co phieu
- da co phieu nhung chua co ledger
- da giam ton nhung chua luu issue line

Day la khai niem `atomicity` o muc nghiep vu.

## 16. Domain model hien tai con thieu gi theo goc business

Domain model hien tai la tot cho core inventory, nhung chua co cac object sau:

1. `StockAdjustment`
2. `Supplier`
3. `Customer`
4. `AuditLog`
5. `Approval`
6. `Warehouse`
7. `BinLocation`

Nhin tu goc business, day la cac mo rong tu nhien cua mo hinh hien tai.

## 17. Cach nhin relationship de hoc business

Khi nhin domain model, ban co the tu hoi:

1. Entity nay la `master`, `transaction`, hay `history`
2. Thuoc tinh nao la state hien tai
3. Thuoc tinh nao la context cua giao dich
4. Quan he nao cho biet business flow

Vi du:

- `Product -> ReceiptLines`
  - cho biet product tung duoc nhap nhieu lan
- `Product -> IssueLines`
  - cho biet product tung duoc xuat nhieu lan
- `Product -> LedgerEntries`
  - cho biet product co lich su bien dong rieng

## 18. Cach doc code theo huong business

Neu muon doc code ma van giu duoc business context, nen doc theo thu tu:

1. `Shared/Domain`
2. `Shared/Contracts`
3. `Server/Controllers`
4. `Server/Services/InventoryService`
5. `Client/Pages`

Ly do:

- Domain cho biet he thong dang mo hinh hoa the gioi nao
- Contracts cho biet he thong giao tiep the nao
- Controller cho biet use case nao ton tai
- Service cho biet logic business chay ra sao
- UI cho biet nguoi dung su dung nghiep vu nhu the nao

## 19. Mo hinh nhan thuc 3 lop de khong roi

Khi hoc domain-driven business app, hay nho 3 lop:

### Lop 1 - `What exists`

Do la entities:

- Category
- Product
- Receipt
- Issue
- Ledger

### Lop 2 - `What users ask the system to do`

Do la requests:

- CreateProductRequest
- CreateStockReceiptRequest
- CreateStockIssueRequest

### Lop 3 - `What the system shows back`

Do la DTO:

- ProductDto
- StockReceiptDetailDto
- InventorySummaryDto

Neu tach duoc 3 lop nay trong dau, ban se doc code business nhanh hon rat nhieu.

## 20. Ket luan

Domain model la noi business duoc "dong bang" thanh cau truc he thong.

Tom gon:

- entity cho thay the gioi nghiep vu duoc mo hinh hoa nhu the nao
- request cho thay nguoi dung muon he thong lam gi
- dto cho thay he thong tra lai thong tin gi
- service cho thay business rule duoc thuc thi nhu the nao

Neu ban hieu domain model, ban se khong chi biet du an "co nhung file nao".  
Ban se biet:

- tai sao file do ton tai
- no dai dien cho khoanh nghiep vu nao
- va no tham gia vao luong kinh doanh nao cua he thong
