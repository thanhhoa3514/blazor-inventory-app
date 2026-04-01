# Tai Lieu Nghiep Vu Toan He Thong Quan Ly Kho

## 1. Muc dich tai lieu

Tai lieu nay mo ta nghiep vu tong the cua he thong quan ly kho `MyApp Inventory`.

Muc tieu la:

- Giai thich ro he thong dang quan ly cai gi.
- Trinh bay tung phan he thong theo goc nhin nghiep vu.
- Chi ra du lieu nao la du lieu master, du lieu nao la du lieu giao dich.
- Mo ta luong nghiep vu tu khi tao danh muc san pham den khi nhap kho, xuat kho, va xem ton kho.
- Giai thich quy tac tinh ton, gia von binh quan, va canh bao low stock.
- Tao ra mot tai lieu de doc xong co the hieu duoc toan bo du an.

Tai lieu nay di cung voi tai lieu auth:

- [authentication-authorization-business-flow.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/authentication-authorization-business-flow.md)

Tai lieu auth tra loi cau hoi:

- Ai dang nhap
- Ai duoc quyen lam gi

Tai lieu nay tra loi cau hoi:

- He thong kho quan ly doi tuong nao
- Moi doi tuong dong vai tro gi trong nghiep vu
- Du lieu thay doi nhu the nao qua moi giao dich

## 2. Pham vi nghiep vu hien tai

He thong hien tai bao gom 5 khoi nghiep vu chinh:

1. `Categories`
2. `Products`
3. `Receipts`
4. `Issues`
5. `Inventory Summary`

Ngoai ra, o tang du lieu he thong con co:

- `Inventory Ledger`

`Inventory Ledger` hien chua co man hinh rieng, nhung da la phan rat quan trong trong nghiep vu vi no giu lich su bien dong ton kho.

## 3. Tong quan mo hinh nghiep vu

He thong duoc to chuc theo 2 nhom du lieu lon:

### 3.1. Du lieu master

Du lieu master la du lieu nen tang, duoc tao truoc va su dung lai nhieu lan:

- `Category`
- `Product`

### 3.2. Du lieu giao dich

Du lieu giao dich la du lieu phat sinh trong qua trinh van hanh kho:

- `StockReceipt`
- `StockIssue`
- `InventoryLedgerEntry`

Nguyen tac nghiep vu:

- Master data xac dinh "co cai gi trong kho".
- Transaction data xac dinh "hang da di vao hoac di ra nhu the nao".

## 4. Bieu dien nghiep vu tong the

Luong nghiep vu tong the cua he thong co the tom tat nhu sau:

1. Admin tao `Category`
2. Admin tao `Product`
3. WarehouseStaff hoac Admin tao `Receipt` de nhap hang vao kho
4. He thong tang ton kho va cap nhat gia von binh quan
5. WarehouseStaff hoac Admin tao `Issue` de xuat hang ra kho
6. He thong giam ton kho va giu nguyen logic gia von xuat theo average cost hien hanh
7. Dashboard `Inventory Summary` tong hop ton kho hien tai va danh sach san pham sap het hang
8. `Inventory Ledger` ghi lai lich su bien dong cho tung giao dich

## 5. Khoi nghiep vu Categories

### 5.1. Muc dich nghiep vu

`Category` dung de nhom cac san pham co tinh chat giong nhau.

Vi du:

- Electronics
- Office Supplies
- Components
- Packaging

Y nghia nghiep vu:

- Giup sap xep danh muc hang hoa ro rang
- Ho tro tim kiem, bao cao, thong ke
- Ho tro dashboard nhin nhom san pham de phan tich

### 5.2. Thong tin nghiep vu cua Category

Moi `Category` hien tai gom:

- `Id`
- `Name`
- `Description`
- `CreatedAtUtc`

### 5.3. Quy tac nghiep vu

1. Ten category la bat buoc.
2. Ten category toi da 100 ky tu.
3. Mo ta toi da 300 ky tu.
4. Ten category phai duy nhat.
5. Khong duoc xoa category neu van con product dang gan vao category do.

### 5.4. Ly do cua quy tac

Ten category duy nhat de:

- tranh trung lap
- tranh bao cao bi vo nghia do cung mot nhom nhung nhap nhieu ten khac nhau

Cam xoa category khi con product de:

- tranh mat lien ket nghiep vu
- tranh product bi roi vao trang thai khong xac dinh nhom

### 5.5. Luong nghiep vu Category

#### Luong tao category

1. Admin mo man `Categories`
2. Nhap ten va mo ta
3. He thong kiem tra du lieu
4. Neu hop le thi tao category
5. Category moi co the duoc dung de tao product

#### Luong sua category

1. Admin chon category can sua
2. He thong nap du lieu hien tai
3. Admin cap nhat ten hoac mo ta
4. He thong kiem tra trung ten
5. Neu hop le thi luu thay doi

#### Luong xoa category

1. Admin chon xoa category
2. He thong kiem tra category co product hay khong
3. Neu co product:
   - tu choi xoa
4. Neu khong co product:
   - cho phep xoa

### 5.6. Gia tri nghiep vu

`Category` la tang phan loai. Neu kho nay khong co, danh muc san pham se roi rac, kho tim, kho thong ke, va kho mo rong.

## 6. Khoi nghiep vu Products

### 6.1. Muc dich nghiep vu

`Product` la doi tuong trung tam cua he thong kho.

Moi giao dich nhap kho va xuat kho deu xoay quanh `Product`.

Neu nhin nghiep vu o muc co ban:

- Category tra loi "nhom nao"
- Product tra loi "cu the la mat hang nao"

### 6.2. Thong tin nghiep vu cua Product

Moi `Product` hien tai gom:

- `Id`
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

### 6.3. Y nghia tung thuoc tinh

#### `Sku`

Ma dinh danh nghiep vu cua san pham.

Vai tro:

- tim kiem nhanh
- doi chieu chung tu
- truy vet san pham

#### `OnHandQty`

So luong ton hien tai trong kho.

Day la gia tri duoc cap nhat sau moi `Receipt` va `Issue`.

#### `AverageCost`

Gia von binh quan hien tai cua san pham.

He thong dung gia tri nay de:

- tinh gia tri ton kho
- tinh line total khi xuat kho
- tinh tong gia tri issue

#### `ReorderLevel`

Nguong canh bao sap het hang.

Neu `OnHandQty <= ReorderLevel` thi san pham duoc xem la low stock.

#### `IsActive`

Trang thai hoat dong cua san pham.

Y nghia nghiep vu:

- `true`: san pham dang duoc phep su dung trong giao dich
- `false`: san pham tam ngung hoac ngung van hanh

### 6.4. Quy tac nghiep vu

1. `Sku` la bat buoc.
2. `Sku` toi da 50 ky tu.
3. `Sku` phai duy nhat.
4. `Name` la bat buoc.
5. `Name` toi da 200 ky tu.
6. `Description` toi da 500 ky tu.
7. `CategoryId` phai ton tai.
8. `ReorderLevel` khong am.
9. `OnHandQty` khong am.
10. Product moi tao co:
    - `OnHandQty = 0`
    - `AverageCost = 0`
    - `IsActive = true`
11. Khong duoc xoa product neu product da co lich su giao dich.
12. Chi product `IsActive = true` moi duoc dua vao giao dich nhap/xuat.

### 6.5. Ly do nghiep vu cua cac quy tac

`Sku` duy nhat de:

- moi san pham duoc xac dinh ro
- tranh nham lan khi nhap xuat
- tranh trung lap trong chung tu

Cam xoa product da co giao dich de:

- giu toan ven lich su kho
- khong lam hong bao cao cu
- giu lien ket voi ledger

Chi cho product active vao giao dich de:

- tranh dung nham mat hang da ngung van hanh
- giu quy trinh kho gon gang

### 6.6. Luong nghiep vu Product

#### Luong tao product

1. Admin vao man `Products`
2. Nhap SKU, Name, Description, Category, ReorderLevel
3. He thong kiem tra category ton tai
4. He thong kiem tra SKU duy nhat
5. Neu hop le thi tao product
6. Product moi co ton = 0 va average cost = 0

#### Luong sua product

1. Admin chon product can sua
2. He thong nap thong tin hien tai
3. Admin cap nhat thong tin
4. He thong kiem tra SKU trung va category hop le
5. Neu hop le thi luu thay doi

#### Luong xoa product

1. Admin chon xoa product
2. He thong kiem tra product da co receipt line hoac issue line chua
3. Neu da co giao dich:
   - khong cho xoa
4. Neu chua co giao dich:
   - cho phep xoa

### 6.7. Gia tri nghiep vu

`Product` la hat nhan cua he thong. Toan bo ton kho, gia tri ton kho, nhap xuat, low stock deu duoc tinh tu bang product va lich su giao dich cua no.

## 7. Khoi nghiep vu Receipts

### 7.1. Muc dich nghiep vu

`StockReceipt` la nghiep vu nhap hang vao kho.

Moi receipt lam:

- tang so luong ton
- tang gia tri ton kho
- co the lam thay doi average cost

### 7.2. Khi nao can Receipt

Receipt duoc dung khi:

- mua hang ve kho
- nhan hang tu nha cung cap
- nhap bo sung hang
- nhan tra hang ve kho trong pham vi nghiep vu hien tai

### 7.3. Cau truc nghiep vu cua Receipt

Mot `Receipt` gom:

- thong tin dau phieu
- nhieu dong chi tiet

Thong tin dau phieu:

- `DocumentNo`
- `ReceivedAtUtc`
- `Supplier`
- `Note`
- `TotalAmount`

Moi dong chi tiet gom:

- `ProductId`
- `Quantity`
- `UnitCost`
- `LineTotal`

### 7.4. Quy tac nghiep vu Receipt

1. Mot receipt phai co it nhat 1 line.
2. Moi line phai co product hop le.
3. Product phai ton tai va dang active.
4. `Quantity` phai > 0.
5. `UnitCost` khong duoc am.
6. `LineTotal = Quantity * UnitCost`, duoc lam tron 2 chu so thap phan.
7. `TotalAmount` cua phieu bang tong cac line total.
8. Moi receipt sinh ra mot `DocumentNo` duy nhat theo timestamp.
9. Receipt duoc xu ly trong transaction de bao dam toan ven du lieu.

### 7.5. Logic nghiep vu khi nhap kho

Day la logic quan trong nhat cua receipt:

Voi moi line:

1. Lay product hien tai.
2. Tinh:
   - `lineTotal = quantity * unitCost`
   - `currentValue = onHandQty * averageCost`
   - `newQty = onHandQty + quantity`
   - `newValue = currentValue + lineTotal`
   - `newAverage = newValue / newQty`
3. Cap nhat product:
   - `OnHandQty = newQty`
   - `AverageCost = newAverage`
4. Tao ledger entry ghi nhan giao dich nhap.

### 7.6. Y nghia nghiep vu cua average cost khi nhap

He thong dang ap dung `weighted average cost`.

Nghia la:

- Moi lan nhap hang moi vao, average cost duoc tinh lai dua tren ton hien tai va gia tri lo hang moi.
- Day la cach tinh phu hop cho he thong kho tong hop co quy mo nho va trung binh.

Vi du:

- Ton hien tai: 10 sp, average cost 100
- Gia tri ton hien tai: 1000
- Nhap them: 5 sp, unit cost 130
- Gia tri nhap them: 650
- Ton moi: 15
- Gia tri moi: 1650
- Average cost moi: 110

### 7.7. Luong nghiep vu Receipt

1. WarehouseStaff mo man `Receipts`
2. Nhap supplier va note neu can
3. Them cac line san pham
4. Kiem tra lai tong gia tri
5. Xac nhan submit
6. He thong validate request
7. He thong cap nhat ton va average cost
8. He thong tao phieu va ledger
9. He thong tra ve chi tiet receipt vua tao

### 7.8. Gia tri nghiep vu

Receipt la diem vao cua hang hoa. Neu nghiep vu nay sai, ton kho va gia tri ton kho se sai tu goc.

## 8. Khoi nghiep vu Issues

### 8.1. Muc dich nghiep vu

`StockIssue` la nghiep vu xuat hang ra kho.

Moi issue lam:

- giam so luong ton
- giam gia tri ton kho
- tao dau vet trong ledger

### 8.2. Khi nao can Issue

Issue duoc dung khi:

- ban hang
- cap phat hang noi bo
- xuat hang cho bo phan khac
- xuat giao cho khach hang

### 8.3. Cau truc nghiep vu cua Issue

Thong tin dau phieu:

- `DocumentNo`
- `IssuedAtUtc`
- `Customer`
- `Note`
- `TotalAmount`

Moi dong chi tiet:

- `ProductId`
- `Quantity`
- `UnitCost`
- `LineTotal`

Khac biet quan trong so voi receipt:

- Khi tao issue, `UnitCost` khong do nguoi dung nhap vao.
- He thong lay `AverageCost` hien tai cua product lam `UnitCost` xuat.

### 8.4. Quy tac nghiep vu Issue

1. Mot issue phai co it nhat 1 line.
2. Product phai ton tai va active.
3. `Quantity` phai > 0.
4. Product phai du ton de xuat.
5. `UnitCost` cua issue = `AverageCost` hien tai cua product.
6. `LineTotal = Quantity * AverageCost`.
7. `TotalAmount` cua phieu bang tong cac line total.
8. Moi issue sinh `DocumentNo` duy nhat theo timestamp.
9. Toan bo issue duoc xu ly trong transaction.

### 8.5. Logic nghiep vu khi xuat kho

Voi moi line:

1. Lay product hien tai.
2. Kiem tra `OnHandQty >= quantity xuat`.
3. Lay `unitCost = AverageCost`.
4. Tinh `lineTotal = quantity * unitCost`.
5. Giam ton:
   - `OnHandQty = OnHandQty - quantity`
6. `AverageCost` hien tai duoc giu nguyen sau issue.
7. Tao ledger entry voi:
   - quantity am
   - value change am

### 8.6. Vi sao average cost khong doi khi issue

Theo logic weighted average:

- Nhap kho moi lam thay doi average cost
- Xuat kho chi giam ton theo average cost hien co

Day la quy tac nghiep vu dung va phu hop voi cach he thong dang thuc thi.

### 8.7. Luong nghiep vu Issue

1. WarehouseStaff mo man `Issues`
2. Chon customer va note neu can
3. Them cac line product can xuat
4. Xac nhan submit
5. He thong validate ton kho
6. Neu du ton:
   - tao issue
   - giam ton
   - ghi ledger
7. Neu khong du ton:
   - tu choi giao dich
   - bao loi ro san pham nao khong du

### 8.8. Gia tri nghiep vu

Issue la diem ra cua hang hoa. Neu nghiep vu nay sai, ton kho co the am, bao cao sai, va kho se mat kha nang kiem soat thuc te.

## 9. Khoi nghiep vu Inventory Summary

### 9.1. Muc dich nghiep vu

`Inventory Summary` la man tong hop tinh hinh kho tai thoi diem hien tai.

No khong tao giao dich, nhung la man ra quyet dinh.

### 9.2. Cac chi so hien tai

`Inventory Summary` hien tai gom:

- `TotalProducts`
- `TotalOnHandUnits`
- `TotalInventoryValue`
- `LowStockCount`
- `LowStockItems`

### 9.3. Y nghia nghiep vu tung chi so

#### `TotalProducts`

Tong so san pham dang active duoc dua vao bao cao.

#### `TotalOnHandUnits`

Tong tat ca so luong ton hien tai cua cac product active.

#### `TotalInventoryValue`

Tong gia tri ton kho hien tai.

Cong thuc:

- `sum(OnHandQty * AverageCost)` tren tat ca product active

#### `LowStockCount`

So san pham co ton thap hon hoac bang nguong dat hang lai.

Cong thuc:

- dem so product co `OnHandQty <= ReorderLevel`

#### `LowStockItems`

Danh sach chi tiet cac san pham sap het hang.

Thong tin gom:

- ProductId
- Sku
- Name
- OnHandQty
- ReorderLevel
- CategoryName

### 9.4. Quy tac nghiep vu

1. Chi tinh tren cac product active.
2. San pham duoc xem la low stock khi `OnHandQty <= ReorderLevel`.
3. Danh sach low stock duoc sap xep:
   - ton thap hon len truoc
   - neu bang nhau thi theo ten

### 9.5. Gia tri nghiep vu

Inventory Summary phuc vu:

- quan ly kho nhin nhanh tinh hinh ton
- nhan dien san pham can nhap bo sung
- uoc tinh gia tri hang dang nam trong kho
- ho tro dieu hanh hang ngay

## 10. Khoi nghiep vu Inventory Ledger

### 10.1. Muc dich nghiep vu

`Inventory LedgerEntry` la nhat ky bien dong ton kho.

Day la lop du lieu de truy vet.

### 10.2. Khi nao ledger duoc tao

Moi khi co:

- `Receipt`
- `Issue`

He thong tao ledger entry cho tung line.

### 10.3. Thong tin nghiep vu cua Ledger

Moi ledger entry gom:

- `ProductId`
- `MovementType`
- `ReferenceNo`
- `OccurredAtUtc`
- `QuantityChange`
- `UnitCost`
- `ValueChange`
- `RunningOnHandQty`
- `RunningAverageCost`

### 10.4. Y nghia nghiep vu

Ledger tra loi cac cau hoi:

- Tai sao ton cua san pham thay doi
- Ton thay doi boi chung tu nao
- Sau giao dich nay ton con bao nhieu
- Sau giao dich nay average cost la bao nhieu

### 10.5. Gia tri nghiep vu

Ledger la nen tang cho:

- stock card
- doi soat kho
- audit trail
- truy vet sai lech

Hien tai he thong da ghi ledger, nhung chua co man hinh hien thi rieng. Day la huong mo rong rat hop ly ve sau.

## 11. Moi quan he nghiep vu giua cac khoi

Moi quan he giua cac thanh phan co the hieu nhu sau:

- Mot `Category` co nhieu `Product`
- Mot `Product` thuoc mot `Category`
- Mot `Receipt` co nhieu `ReceiptLine`
- Mot `Issue` co nhieu `IssueLine`
- Moi `ReceiptLine` va `IssueLine` deu gan voi mot `Product`
- Moi `ReceiptLine` va `IssueLine` tao ra `InventoryLedgerEntry`

Neu viet bang ngon ngu nghiep vu:

- Category to chuc product
- Product la trung tam ton kho
- Receipt va Issue tao bien dong
- Ledger giu lich su bien dong
- Summary tong hop trang thai hien tai

## 12. Chu trinh doi song nghiep vu cua mot Product

De de hieu du an, co the nhin theo vong doi song cua mot product:

### Giai doan 1 - Khoi tao

1. Admin tao category
2. Admin tao product
3. Product xuat hien trong he thong voi ton = 0

### Giai doan 2 - Dua vao van hanh

1. WarehouseStaff tao receipt dau tien
2. Product bat dau co ton kho
3. Average cost duoc hinh thanh

### Giai doan 3 - Van hanh thuong xuyen

1. Tiep tuc co them receipt
2. Phat sinh issue
3. OnHandQty thay doi lien tuc
4. AverageCost thay doi khi nhap, giu nguyen khi xuat

### Giai doan 4 - Canh bao

1. Ton giam dan
2. Khi `OnHandQty <= ReorderLevel`
3. Product xuat hien trong low stock list

### Giai doan 5 - Ngung su dung

1. Admin co the set `IsActive = false`
2. Product khong con duoc dua vao giao dich moi
3. Du lieu cu van duoc giu lai de tham chieu va bao cao

## 13. Cac quy tac tinh toan nghiep vu quan trong

### 13.1. Tinh line total trong receipt

Cong thuc:

- `LineTotal = Quantity * UnitCost`

### 13.2. Tinh total amount cua receipt

Cong thuc:

- `TotalAmount = tong tat ca line total`

### 13.3. Tinh average cost moi sau receipt

Cong thuc:

- `CurrentValue = OnHandQty hien tai * AverageCost hien tai`
- `NewValue = CurrentValue + gia tri lo hang moi`
- `NewQty = ton hien tai + so luong nhap`
- `NewAverage = NewValue / NewQty`

### 13.4. Tinh line total trong issue

Cong thuc:

- `LineTotal = Quantity * AverageCost hien tai`

### 13.5. Tinh total amount cua issue

Cong thuc:

- `TotalAmount = tong tat ca line total`

### 13.6. Tinh tong gia tri ton kho

Cong thuc:

- `TotalInventoryValue = tong (OnHandQty * AverageCost)` cua tat ca product active

## 14. Cac rang buoc nghiep vu va du lieu

### 14.1. Rang buoc tren Category

- Name required
- Name unique
- Name max 100
- Description max 300

### 14.2. Rang buoc tren Product

- Sku required
- Sku unique
- Name required
- Category phai ton tai
- ReorderLevel >= 0
- OnHandQty >= 0

### 14.3. Rang buoc tren Receipt

- Co it nhat 1 line
- Quantity > 0
- UnitCost >= 0
- Product phai active

### 14.4. Rang buoc tren Issue

- Co it nhat 1 line
- Quantity > 0
- Product phai active
- Khong duoc xuat vuot ton

## 15. Cac tinh huong nghiep vu canh bien

### 15.1. Tao product nhung chon category khong ton tai

Ket qua nghiep vu:

- Tu choi tao product

Ly do:

- Product phai thuoc mot category hop le

### 15.2. Xoa category khi van con product

Ket qua nghiep vu:

- Tu choi xoa

Ly do:

- Dam bao toan ven master data

### 15.3. Xoa product da co giao dich

Ket qua nghiep vu:

- Tu choi xoa

Ly do:

- Neu xoa se vo lich su nghiep vu

### 15.4. Tao receipt voi quantity am hoac 0

Ket qua nghiep vu:

- Tu choi submit

Ly do:

- Nhap kho phai tang ton bang mot so duong

### 15.5. Tao issue vuot ton

Ket qua nghiep vu:

- Tu choi submit

Ly do:

- Kho khong du hang de xuat

### 15.6. Dung product inactive trong giao dich

Ket qua nghiep vu:

- Tu choi submit

Ly do:

- Product inactive duoc xem la khong con hop le cho van hanh moi

## 16. Goc nhin bao cao va kiem soat

Neu nhin he thong theo quan diem quan tri kho, hien tai co 3 lop nhin:

### 16.1. Lop master

- Categories
- Products

Tra loi:

- Kho dang quan ly nhung mat hang nao

### 16.2. Lop transaction

- Receipts
- Issues

Tra loi:

- Hang da di vao va di ra nhu the nao

### 16.3. Lop summary

- Inventory Summary

Tra loi:

- Kho dang o trang thai nao ngay luc nay

### 16.4. Lop traceability

- Inventory Ledger

Tra loi:

- Tai sao kho dang o trang thai nhu vay

## 17. Mapping nghiep vu sang man hinh

### 17.1. `/categories`

Dung de:

- xem category
- tao, sua, xoa category

### 17.2. `/products`

Dung de:

- xem product
- tao, sua, xoa product
- theo doi on hand, average cost, reorder level

### 17.3. `/receipts`

Dung de:

- tao phieu nhap
- xem lich su phieu nhap

### 17.4. `/receipts/{id}`

Dung de:

- xem chi tiet tung phieu nhap

### 17.5. `/issues`

Dung de:

- tao phieu xuat
- xem lich su phieu xuat

### 17.6. `/issues/{id}`

Dung de:

- xem chi tiet tung phieu xuat

### 17.7. `/`

Dung de:

- xem inventory summary
- xem low stock

## 18. Thu tu hieu du an de khong bi roi

Neu ban muon hieu du an theo thu tu de nhat, nen doc va suy nghi theo cau truc sau:

1. Hieu `Category` truoc
2. Sau do hieu `Product`
3. Sau do hieu `Receipt`
4. Sau do hieu `Issue`
5. Cuoi cung hieu `Inventory Summary` va `Ledger`

Vi sao?

- Khong co category thi chua co product
- Khong co product thi chua co nhap xuat
- Khong co nhap xuat thi chua co ton
- Khong co ton thi summary khong co y nghia

## 19. Nhung gi he thong da lam dung o goc nghiep vu

Phien ban hien tai da co nhieu diem nghiep vu dung:

1. Tach ro master data va transaction data
2. Khong cho xoa master data mot cach vo toi va
3. Tinh average cost theo weighted average
4. Chan xuat vuot ton
5. Co low stock theo reorder level
6. Co ledger de truy vet bien dong
7. Co auth va RBAC de tach quyen van hanh

Day la bo khung tot cho mot he thong kho MVP hoac small business inventory app.

## 20. Nhung khoang trong nghiep vu hien tai

Du an hien tai van chua co mot so nghiep vu quan trong neu muon di xa hon:

1. `Stock Adjustment`
   - dieu chinh ton sau kiem ke
2. `Supplier master`
   - hien tai supplier dang la text
3. `Customer master`
   - hien tai customer dang la text
4. `Stock Card UI`
   - da co ledger nhung chua co man hinh
5. `Audit theo user`
   - giao dich chua gan ro nguoi tao
6. `Purchase Order / Sales Order`
   - chua co workflow truoc khi nhap/xuat
7. `Multi-warehouse`
   - hien dang la 1 kho logic duy nhat

## 21. Ket luan

Toan bo he thong co the duoc hieu ngan gon nhu sau:

- `Categories` to chuc nhom hang
- `Products` dinh nghia tung mat hang va trang thai ton hien tai
- `Receipts` dua hang vao kho
- `Issues` dua hang ra kho
- `Inventory Summary` tong hop tinh hinh kho hien tai
- `Inventory Ledger` ghi lai toan bo bien dong de truy vet

Neu can nam du an mot cach ro rang, ban co the ghi nho 4 cau:

1. `Product` la trung tam cua kho.
2. `Receipt` lam tang ton va co the doi average cost.
3. `Issue` lam giam ton va xuat theo average cost hien tai.
4. `Summary` la anh chup hien tai, con `Ledger` la lich su tao ra buc anh do.

Tai lieu nay, ket hop voi tai lieu auth, da bao phu hai cau hoi lon nhat cua du an:

- He thong nay cho ai dung va moi nguoi duoc lam gi
- He thong nay quan ly kho nhu the nao va du lieu bien dong ra sao
