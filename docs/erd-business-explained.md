# ERD Business Explained - Giai Thich So Do Du Lieu Theo Goc Nhin Nghiep Vu

## 1. Muc dich tai lieu

Tai lieu nay giai thich mo hinh du lieu cua du an theo goc nhin business.

Muc tieu:

- giup ban nhin bang du lieu nhu cac khai niem nghiep vu
- giai thich vi sao bang nay lien ket voi bang kia
- giup ban doc ERD khong chi nhu quan he khoa ngoai, ma nhu dong chay nghiep vu

## 2. Cach nhin ERD theo business

Khi nhin ERD, co 3 cau hoi can dat:

1. Bang nay dai dien cho doi tuong nghiep vu nao
2. Tai sao no phai lien ket voi bang kia
3. Neu bo lien ket do di thi quy trinh business nao se bi vo

## 3. Cac bang/doi tuong chinh

He thong hien tai co the duoc nhin qua 7 doi tuong:

1. `Category`
2. `Product`
3. `StockReceipt`
4. `StockReceiptLine`
5. `StockIssue`
6. `StockIssueLine`
7. `InventoryLedgerEntry`

Ngoai ra, sau khi co auth:

8. `AspNetUsers`
9. `AspNetRoles`
10. `AspNetUserRoles`

## 4. Category va Product

### Quan he

- 1 `Category` co nhieu `Product`
- 1 `Product` thuoc 1 `Category`

### Y nghia business

Category la lop phan loai.

Product la mat hang cu the.

Doanh nghiep can phan loai hang hoa theo nhom de:

- de tim
- de thong ke
- de bao cao

### Neu khong co quan he nay

- product se khong co nhom
- bao cao theo category se khong ton tai
- master data se mat tinh to chuc

## 5. Product va StockReceiptLine

### Quan he

- 1 `Product` co the xuat hien trong nhieu `StockReceiptLine`
- 1 `StockReceiptLine` chi thuoc ve 1 `Product`

### Y nghia business

Mot mat hang co the duoc nhap nhieu lan, moi lan la mot phan cua mot receipt nao do.

Day phan anh nghiep vu:

- product duoc bo sung ton nhieu dot

### Vi sao khong lien ket product truc tiep voi receipt

Vi 1 receipt co nhieu product.  
Can bang line de dai dien cho tung dong chi tiet.

## 6. StockReceipt va StockReceiptLine

### Quan he

- 1 `StockReceipt` co nhieu `StockReceiptLine`
- 1 `StockReceiptLine` thuoc 1 `StockReceipt`

### Y nghia business

Day la mo hinh `header-detail`.

Header giu:

- doc no
- ngay nhap
- supplier
- note
- total

Detail giu:

- product nao
- so luong bao nhieu
- gia nhap bao nhieu

### Vi sao can tach header va detail

Vi business document thuc te thuong duoc lap theo kieu:

- 1 phieu
- nhieu dong hang

## 7. Product va StockIssueLine

### Quan he

- 1 `Product` co the xuat hien trong nhieu `StockIssueLine`
- 1 `StockIssueLine` thuoc 1 `Product`

### Y nghia business

Mot product co the duoc xuat nhieu lan trong suot doi song cua no.

Quan he nay giup he thong:

- truy lich su xuat cua tung product

## 8. StockIssue va StockIssueLine

### Quan he

- 1 `StockIssue` co nhieu `StockIssueLine`
- 1 `StockIssueLine` thuoc 1 `StockIssue`

### Y nghia business

Giong receipt, issue cung la mo hinh header-detail.

Business doc:

- dau phieu xuat
- cac dong hang xuat

## 9. Product va InventoryLedgerEntry

### Quan he

- 1 `Product` co nhieu `InventoryLedgerEntry`
- 1 `InventoryLedgerEntry` thuoc 1 `Product`

### Y nghia business

Moi bien dong cua product deu de lai dau vet.

Quan he nay giup tra loi:

- product nay da bien dong the nao tu truoc den nay

### Gia tri business

Day la nen tang cho:

- stock card
- audit giao dich
- truy vet sai lech

## 10. Moi lien he gian tiep giua Receipt/Issue va Product

Co mot diem quan trong:

- `Receipt` khong lien ket truc tiep den `Product`
- `Issue` khong lien ket truc tiep den `Product`

No lien ket qua bang line.

### Ly do business

Vi 1 receipt/issue khong chi chua 1 product.  
No chua nhieu product.

Neu lien ket truc tiep:

- se khong model duoc chung tu nhieu dong

## 11. Product la trung tam cua ERD nghiep vu

Neu nhin ERD tu goc business, `Product` la nut trung tam nhat.

No lien ket voi:

- `Category`
- `ReceiptLine`
- `IssueLine`
- `InventoryLedgerEntry`

Vi sao?

Vi inventory system thuc chat la he thong theo doi:

- mot mat hang
- qua cac bien dong
- theo thoi gian

## 12. InventoryLedgerEntry la cau noi giua lich su va trang thai

### Y nghia business

Product luu trang thai hien tai:

- OnHandQty
- AverageCost

Ledger luu lich su tao ra trang thai do.

### Neu khong co ledger

He thong van van hanh duoc o muc co ban, nhung se kho:

- truy vet
- giai thich sai lech
- lam stock card

## 13. Role/auth tables trong business

### AspNetUsers

Dai dien cho:

- nguoi su dung he thong

### AspNetRoles

Dai dien cho:

- nhom vai tro nghiep vu

### AspNetUserRoles

Dai dien cho:

- user nao thuoc role nao

### Y nghia business

Day la lop kiem soat:

- ai duoc thao tac
- ai chi duoc xem

Neu khong co no:

- he thong kho se mo va thieu kiem soat noi bo

## 14. ERD o goc nhin dong chay nghiep vu

Co the doc ERD theo chuoi sau:

1. Tao `Category`
2. Tao `Product` thuoc category do
3. Tao `Receipt`
4. Tao `ReceiptLine` cho tung product duoc nhap
5. Cap nhat `Product.OnHandQty` va `Product.AverageCost`
6. Tao `InventoryLedgerEntry`
7. Tao `Issue`
8. Tao `IssueLine`
9. Giam `Product.OnHandQty`
10. Tao them `InventoryLedgerEntry`

Day la cach ERD khong chi "nam yen".  
No song cung business flow.

## 15. Business meaning cua cac khoa ngoai

Khoa ngoai trong business khong chi de join.  
No the hien "su phu thuoc nghiep vu".

### Vi du 1

`Product.CategoryId`

Nghia la:

- product phai thuoc mot category hop le

### Vi du 2

`StockReceiptLine.ProductId`

Nghia la:

- moi dong nhap phai noi ro nhap mat hang nao

### Vi du 3

`InventoryLedgerEntry.ProductId`

Nghia la:

- moi bien dong ton kho phai gan voi mot product cu the

## 16. Business meaning cua rang buoc xoa

Trong mo hinh hien tai:

- khong de xoa category neu con product
- khong de xoa product neu con lich su giao dich

Day khong chi la ky thuat.  
Day la business protection.

No ngan:

- mat ngu canh danh muc
- vo lich su giao dich

## 17. ERD hien tai con thieu gi neu mo rong business

Neu he thong phat trien tiep, ERD se co the them:

1. `Supplier`
2. `Customer`
3. `StockAdjustment`
4. `AuditLog`
5. `Warehouse`
6. `BinLocation`
7. `PurchaseOrder`
8. `Approval`

Moi bang moi thuong xuat hien khi business dat them cau hoi moi.

Vi du:

- "ai tao giao dich nay" -> can audit
- "hang nam o kho nao" -> can warehouse
- "vi sao ton chenh" -> can adjustment

## 18. Cach tu ve ERD business cho du an nay

Neu ban tu ve bang tay, co the sap xep thanh 4 cum:

### Cum 1 - Control

- Users
- Roles

### Cum 2 - Master

- Category
- Product

### Cum 3 - Transactions

- Receipt
- ReceiptLine
- Issue
- IssueLine

### Cum 4 - Trace and summary

- InventoryLedgerEntry

Day la cach ve rat de hieu khi hoc business.

## 19. Cau hoi tu kiem tra de hoc ERD business

1. Tai sao Product phai noi voi Category
2. Tai sao Receipt phai co Lines
3. Tai sao Issue khong cap nhat average cost
4. Tai sao Ledger can luu running values
5. Tai sao Product khong nen bi xoa neu da co giao dich
6. Tai sao auth table la mot phan cua business control

Neu tra loi duoc 6 cau nay, ban da hieu ERD cua he thong o muc rat kha.

## 20. Ket luan

ERD business khong chi la so do bang va khoa.  
No la ban do cua cach doanh nghiep mo hinh hoa:

- hang hoa
- giao dich
- lich su
- quyen han

Trong du an nay:

- `Product` la tam diem
- `Receipt` va `Issue` la su kien
- `Ledger` la dau vet
- `Category` la phan loai
- `Users/Roles` la lop kiem soat

Day la cach nhin ERD dung de hoc business system.
