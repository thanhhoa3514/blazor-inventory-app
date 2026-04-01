# Business Glossary - Tu Dien Thuat Ngu Nghiep Vu Quan Ly Kho

## 1. Muc dich tai lieu

Tai lieu nay la tu dien thuat ngu business cho du an `MyApp Inventory`.

Muc tieu:

- giai nghia cac thuat ngu xuat hien trong he thong
- giup ban doc tai lieu va code khong bi vo nghia
- tach bach y nghia nghiep vu va y nghia ky thuat

Khi hoc business system, roi roi thuong xay ra vi mot tu duoc dung o nhieu muc:

- mot tu trong UI
- mot tu trong code
- mot tu trong nghiep vu doanh nghiep

Tai lieu nay giup dong nhat cach hieu.

## 2. Cach doc glossary

Moi muc duoc trinh bay theo 4 phan:

1. `Dinh nghia`
2. `Y nghia business`
3. `Trong du an nay no xuat hien o dau`
4. `Luu y de khong hieu sai`

## 3. Nhom thuat ngu nen tang

### 3.1. Inventory

#### Dinh nghia

`Inventory` la hang hoa, vat tu, thanh pham, ban thanh pham hoac tai san luu kho ma doanh nghiep dang quan ly.

#### Y nghia business

Day la "tai san luan chuyen" trong hoat dong van hanh.

Neu khong quan ly inventory tot:

- de that thoat hang
- de mat don hang
- de mua du hoac mua thieu

#### Trong du an nay

Toan bo du an de giai quyet bai toan inventory control.

#### Luu y

`Inventory` khong chi la "hang dang con trong kho".  
No bao gom ca logic:

- nhap vao
- xuat ra
- gia tri ton
- lich su bien dong

### 3.2. Warehouse

#### Dinh nghia

`Warehouse` la noi doanh nghiep luu tru hang hoa va thuc hien cac hoat dong nhap, xuat, sap xep, kiem dem.

#### Y nghia business

Kho la noi trung gian giua:

- nguon hang dau vao
- nhu cau dau ra

#### Trong du an nay

He thong hien tai dang gia dinh 1 kho logic duy nhat.

#### Luu y

Day chua phai he thong `multi-warehouse`.

### 3.3. Stock

#### Dinh nghia

`Stock` la luong hang ton tai o mot thoi diem.

#### Y nghia business

No tra loi:

- con bao nhieu de dung
- co can nhap them khong

#### Trong du an nay

Stock hien tai duoc bieu dien bang `Product.OnHandQty`.

#### Luu y

`Stock` thuong duoc dung chung chung.  
Trong code hien tai, gia tri cu the la `OnHandQty`.

## 4. Nhom thuat ngu master data

### 4.1. Master Data

#### Dinh nghia

`Master data` la du lieu nen tang, duoc tao mot lan va duoc tham chieu lap di lap lai trong nhieu quy trinh.

#### Y nghia business

Master data la "tu dien chuan" cua he thong.

Neu master data ban:

- giao dich se ban
- bao cao se sai

#### Trong du an nay

Master data hien tai bao gom:

- `Category`
- `Product`

#### Luu y

`Supplier` va `Customer` hien chua tro thanh master data, ma van dang la text tu do.

### 4.2. Category

#### Dinh nghia

`Category` la nhom phan loai san pham.

#### Y nghia business

Giup to chuc danh muc hang hoa theo nhom nghiep vu.

#### Trong du an nay

- bang `Categories`
- man hinh `/categories`

#### Luu y

Category khong phai la don vi ton kho.  
No chi la lop phan loai.

### 4.3. Product

#### Dinh nghia

`Product` la mat hang cu the duoc quan ly trong kho.

#### Y nghia business

Day la doi tuong trung tam cua inventory.

#### Trong du an nay

- bang `Products`
- man hinh `/products`
- la doi tuong duoc tham chieu trong receipt va issue

#### Luu y

Product trong du an nay la mot sku logic, chua co batch, serial, hay don vi quy doi.

### 4.4. SKU

#### Dinh nghia

`SKU` viet tat cua `Stock Keeping Unit`.

#### Y nghia business

La ma dinh danh nghiep vu de nhan biet mot mat hang.

#### Trong du an nay

- `Product.Sku`
- bat buoc unique

#### Luu y

SKU khong nhat thiet phai la barcode.  
No la ma nghiep vu do doanh nghiep dat ra.

### 4.5. IsActive

#### Dinh nghia

`IsActive` cho biet product co con duoc phep dua vao giao dich moi hay khong.

#### Y nghia business

Day la co che "ngung van hanh" mot product ma khong xoa du lieu.

#### Trong du an nay

- `Product.IsActive`
- chi product active moi duoc dua vao receipt/issue

#### Luu y

`Inactive` khong co nghia la "khong ton tai".  
No co nghia la "khong dung cho giao dich moi".

## 5. Nhom thuat ngu ton kho va gia tri kho

### 5.1. On Hand Qty

#### Dinh nghia

`OnHandQty` la so luong ton hien co trong kho tai thoi diem hien tai.

#### Y nghia business

Day la so luong co the duoc xem la ton kho sẵn co.

#### Trong du an nay

- `Product.OnHandQty`

#### Luu y

He thong hien tai chua phan tach:

- reserved quantity
- available to promise

Nen `OnHandQty` dang dong thoi la ton co san theo logic he thong.

### 5.2. Average Cost

#### Dinh nghia

`AverageCost` la gia von binh quan hien tai cua mot product.

#### Y nghia business

Dung de:

- tinh gia tri ton kho
- tinh gia tri xuat kho
- theo doi xu huong gia von

#### Trong du an nay

- `Product.AverageCost`
- cap nhat khi receipt
- duoc su dung khi issue

#### Luu y

He thong dung `weighted average`, khong phai FIFO.

### 5.3. Inventory Value

#### Dinh nghia

`Inventory Value` la tong gia tri ton kho tai thoi diem hien tai.

#### Y nghia business

No cho biet doanh nghiep dang nam giu bao nhieu gia tri trong kho.

#### Trong du an nay

- `InventorySummary.TotalInventoryValue`

#### Cong thuc

- tong cua `OnHandQty * AverageCost`

#### Luu y

Day la gia tri tinh theo logic ton kho, khong phai bao cao ke toan day du.

### 5.4. Reorder Level

#### Dinh nghia

`ReorderLevel` la nguong ton kho toi thieu de kich hoat canh bao can nhap them.

#### Y nghia business

Neu ton xuong duoi muc nay, doanh nghiep can xem xet bo sung hang.

#### Trong du an nay

- `Product.ReorderLevel`
- dung de tinh low stock

#### Luu y

Reorder level la nguong canh bao, chua phai la so luong de xuat mua.

### 5.5. Low Stock

#### Dinh nghia

`Low stock` la trang thai san pham sap het hang theo quy tac cua doanh nghiep.

#### Y nghia business

Dung de canh bao som, tranh het hang.

#### Trong du an nay

San pham duoc xem la low stock khi:

- `OnHandQty <= ReorderLevel`

#### Luu y

Day la canh bao nghiep vu, khong phai loi he thong.

## 6. Nhom thuat ngu giao dich

### 6.1. Transaction

#### Dinh nghia

`Transaction` la mot su kien nghiep vu lam thay doi du lieu.

#### Y nghia business

Trong kho, transaction la noi du lieu thuc su bien dong.

#### Trong du an nay

Transaction chinh la:

- `Receipt`
- `Issue`

#### Luu y

Trong ky thuat, transaction cung co nghia database transaction.  
Can tach bach:

- `business transaction`
- `database transaction`

### 6.2. Receipt

#### Dinh nghia

`Receipt` la giao dich nhap kho.

#### Y nghia business

No danh dau hang di vao kho.

#### Trong du an nay

- `StockReceipt`
- `StockReceiptLine`
- man hinh `/receipts`

#### Luu y

Receipt lam tang ton va co the thay doi average cost.

### 6.3. Issue

#### Dinh nghia

`Issue` la giao dich xuat kho.

#### Y nghia business

No danh dau hang di ra kho.

#### Trong du an nay

- `StockIssue`
- `StockIssueLine`
- man hinh `/issues`

#### Luu y

Issue lam giam ton, nhung khong tinh lai average cost.

### 6.4. Line Item

#### Dinh nghia

`Line item` la mot dong chi tiet trong mot chung tu.

#### Y nghia business

Moi chung tu co the chua nhieu san pham, va moi san pham la mot dong.

#### Trong du an nay

- `StockReceiptLine`
- `StockIssueLine`

#### Luu y

Phieu la "dau phieu".  
Line item la "dong chi tiet".

### 6.5. Document No

#### Dinh nghia

`DocumentNo` la so chung tu cua giao dich.

#### Y nghia business

Dung de tham chieu, doi soat, truy vet.

#### Trong du an nay

- receipt: `RCPT-yyyyMMddHHmmssfff`
- issue: `ISS-yyyyMMddHHmmssfff`

#### Luu y

Day la so chung tu he thong sinh, khong nhat thiet la so chung tu giay to ngoai doi.

### 6.6. Supplier

#### Dinh nghia

`Supplier` la nha cung cap hoac nguon hang.

#### Y nghia business

Cho biet lo hang nhap den tu dau.

#### Trong du an nay

- `StockReceipt.Supplier`

#### Luu y

Hien tai supplier moi la text, chua la master data.

### 6.7. Customer

#### Dinh nghia

`Customer` la doi tuong nhan hang xuat.

#### Y nghia business

Cho biet hang da di den dau.

#### Trong du an nay

- `StockIssue.Customer`

#### Luu y

Hien tai customer moi la text, chua la master data.

## 7. Nhom thuat ngu tai chinh va gia tri giao dich

### 7.1. Unit Cost

#### Dinh nghia

`UnitCost` la gia tri tren moi don vi san pham trong giao dich.

#### Y nghia business

No cho biet:

- gia nhap tren moi don vi
- hoac gia von xuat tren moi don vi

#### Trong du an nay

- receipt: nguoi dung nhap vao
- issue: he thong lay tu `AverageCost`

#### Luu y

`UnitCost` trong issue khong phai gia ban.  
No la gia von xuat kho.

### 7.2. Line Total

#### Dinh nghia

`LineTotal` la tong gia tri cua mot dong giao dich.

#### Cong thuc

- `Quantity * UnitCost`

#### Trong du an nay

- co trong receipt line va issue line

#### Luu y

Day la tong gia tri nghiep vu cua line, khong phai tong doanh thu.

### 7.3. Total Amount

#### Dinh nghia

`TotalAmount` la tong gia tri cua toan bo chung tu.

#### Cong thuc

- tong tat ca `LineTotal`

#### Trong du an nay

- `StockReceipt.TotalAmount`
- `StockIssue.TotalAmount`

## 8. Nhom thuat ngu truy vet va lich su

### 8.1. Inventory Ledger

#### Dinh nghia

`Inventory Ledger` la nhat ky bien dong ton kho.

#### Y nghia business

No ghi lai moi thay doi quan trong lien quan den ton va gia tri ton.

#### Trong du an nay

- `InventoryLedgerEntry`

#### Luu y

Day la lich su chi tiet, khac voi `Inventory Summary` la tong hop hien tai.

### 8.2. Movement Type

#### Dinh nghia

`MovementType` la loai bien dong kho.

#### Trong du an nay

Hien co:

- `RECEIPT`
- `ISSUE`

#### Y nghia business

Cho biet ton kho thay doi vi ly do gi.

### 8.3. Quantity Change

#### Dinh nghia

`QuantityChange` la so luong tang hoac giam o mot bien dong.

#### Trong du an nay

- duong: nhap kho
- am: xuat kho

#### Luu y

Can phan biet voi `OnHandQty`.

- `QuantityChange`: thay doi cua 1 giao dich
- `OnHandQty`: ton hien tai sau nhieu giao dich

### 8.4. Running On Hand Qty

#### Dinh nghia

`RunningOnHandQty` la ton kho sau khi giao dich do duoc ap dung.

#### Y nghia business

Giup tra loi:

- sau receipt/issue nay thi con bao nhieu

#### Trong du an nay

- `InventoryLedgerEntry.RunningOnHandQty`

### 8.5. Running Average Cost

#### Dinh nghia

`RunningAverageCost` la average cost sau giao dich do.

#### Y nghia business

Giup truy vet gia von thay doi nhu the nao.

#### Trong du an nay

- `InventoryLedgerEntry.RunningAverageCost`

## 9. Nhom thuat ngu role va kiem soat

### 9.1. Authentication

#### Dinh nghia

Qua trinh xac dinh nguoi dang dung he thong la ai.

#### Trong du an nay

- login bang username/email va password

### 9.2. Authorization

#### Dinh nghia

Qua trinh kiem tra user duoc lam gi.

#### Trong du an nay

- dua tren role

### 9.3. RBAC

#### Dinh nghia

`Role-Based Access Control` la co che cap quyen theo vai tro.

#### Trong du an nay

- `Admin`
- `WarehouseStaff`
- `Viewer`

### 9.4. Policy

#### Dinh nghia

`Policy` la nhom quy tac quyen duoc dat ten de tai su dung.

#### Trong du an nay

- `ReadAccess`
- `WarehouseOperations`
- `AdminOnly`

#### Luu y

Role la "ban la ai".  
Policy la "dieu kien nao de duoc lam viec nay".

## 10. Nhom thuat ngu mo rong trong roadmap

### 10.1. Stock Adjustment

#### Dinh nghia

Giao dich dieu chinh ton kho khong xuat phat tu nhap hang hay xuat hang thong thuong.

#### Y nghia business

Dung de xu ly:

- kiem ke lech
- hao hut
- hu hong
- thua hang

### 10.2. Stock Card

#### Dinh nghia

Bao cao hoac man hinh hien toan bo lich su bien dong cua mot product.

#### Y nghia business

Giup doi soat va truy vet.

### 10.3. Audit Trail

#### Dinh nghia

Lich su ghi lai ai da tao, sua, xoa hoac tac dong den du lieu.

#### Y nghia business

Giup kiem soat noi bo va truy trach nhiem.

### 10.4. Soft Delete

#### Dinh nghia

An ban ghi khoi van hanh ma khong xoa vat ly khoi database.

#### Y nghia business

Giu lai lich su nhung khong cho tiep tuc su dung du lieu do.

### 10.5. Reorder Recommendation

#### Dinh nghia

De xuat nghiep vu ve nhung san pham can nhap them.

#### Y nghia business

Chuyen low stock tu canh bao thanh goi y hanh dong.

## 11. Cach tu hoc business bang glossary nay

Moi khi gap mot tu trong:

- UI
- tai lieu
- code
- backlog

Ban nen tu hoi 3 cau:

1. Tu nay dang noi ve `master`, `transaction`, `summary`, hay `control`
2. No la "trang thai hien tai" hay "su kien da xay ra"
3. No co y nghia nghiep vu hay chi la chi tiet ky thuat

Neu tra loi duoc 3 cau nay, ban se hieu he thong ro hon rat nhanh.

## 12. Ket luan

Glossary la cong cu rat quan trong khi hoc business system vi no giup ban:

- dung tu dung nghia
- hieu duoc tai lieu nhanh hon
- noi logic nghiep vu voi code nhanh hon

Noi ngan gon:

- neu tai lieu la ban do
- thi glossary la chu giai cua ban do do
