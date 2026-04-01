# System Overview - Goc Nhin Nghiep Vu Toan Du An

## 1. Muc dich tai lieu

Tai lieu nay dung de nhin du an o muc "he thong kinh doanh", khong chi o muc code.

Neu hai tai lieu truoc tra loi:

- `Authentication va Authorization`: ai duoc lam gi
- `Warehouse business flow`: du lieu kho bien dong nhu the nao

Thi tai lieu nay tra loi:

- He thong nay sinh ra de giai quyet van de kinh doanh nao
- Nguoi dung nao tham gia vao he thong
- Moi bo phan thay he thong co y nghia gi
- Thong tin di chuyen giua cac man hinh va quy trinh ra sao
- Nguoi quan ly can nhin du an nay nhu mot bai toan van hanh nhu the nao

Tai lieu lien quan:

- [authentication-authorization-business-flow.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/authentication-authorization-business-flow.md)
- [warehouse-business-flow.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/warehouse-business-flow.md)
- [api-business-mapping.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/api-business-mapping.md)
- [future-roadmap.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/future-roadmap.md)

## 2. Bai toan kinh doanh ma he thong dang giai quyet

Bat ky don vi nao co kho hang deu gap nhung cau hoi sau:

1. Trong kho hien co nhung mat hang nao
2. Moi mat hang con bao nhieu
3. Gia tri ton kho hien tai la bao nhieu
4. Hang nao sap het de can nhap them
5. Hang nao da nhap vao, nhap luc nao, tu nguon nao
6. Hang nao da xuat ra, xuat cho ai, vao thoi diem nao
7. Ai duoc phep thay doi du lieu va ai duoc phep chi xem

Neu khong co he thong:

- kho de kiem soat ton kho chinh xac
- de sai khi ghi chep bang tay
- de mat dau vet giao dich
- kho ra quyet dinh nhap them hay dung nhap
- kho tach trach nhiem giua nguoi van hanh va nguoi quan ly

He thong `MyApp Inventory` duoc xay ra de giai quyet cac nhu cau do o muc co ban nhung dung nghiep vu.

## 3. Tam nhin nghiep vu cua he thong

He thong nay hien tai la mot he thong `inventory control` hon la mot he thong `supply chain` day du.

No tap trung vao 3 nang luc cot loi:

1. Quan ly danh muc hang hoa
2. Ghi nhan bien dong nhap - xuat
3. Tong hop trang thai ton kho hien tai

No chua phai la:

- he thong mua hang day du
- he thong ban hang day du
- he thong ERP da phong ban
- he thong da kho, da vi tri

Vi vay, khi hoc business qua du an nay, hay hieu no la:

- lop "xuong song" cua van hanh kho
- noi du lieu ton kho duoc tao, thay doi, va tong hop

## 4. Doi tuong kinh doanh tham gia

### 4.1. Admin

Nhieu khi la:

- chu doanh nghiep
- quan tri vien he thong
- inventory manager
- nguoi phu trach master data

Muc tieu nghiep vu:

- thiet lap cau truc danh muc
- kiem soat product
- dam bao du lieu dau vao dung

Muc tieu quan ly:

- tranh du lieu rac
- tranh duplicate SKU
- tranh nhan vien van hanh sua master data tuy tien

### 4.2. WarehouseStaff

Nhieu khi la:

- thu kho
- nhan vien nhap xuat
- nguoi truc tiep thao tac kho hang

Muc tieu nghiep vu:

- nhap hang dung
- xuat hang dung
- cap nhat ton kho kip thoi

Muc tieu van hanh:

- thao tac nhanh
- giam sai line item
- dam bao ton kho sau moi giao dich la chinh xac

### 4.3. Viewer

Nhieu khi la:

- quan ly theo doi bao cao
- ke toan
- bo phan kinh doanh
- bo phan mua hang

Muc tieu nghiep vu:

- xem tinh hinh ton
- xem lich su nhap xuat
- ra quyet dinh dua tren du lieu

Muc tieu quan tri:

- co quyen xem nhung khong duoc lam thay doi du lieu van hanh

## 5. Gia tri kinh doanh ma he thong tao ra

### 5.1. Gia tri van hanh

- biet duoc ton kho hien tai
- giam sai sot so lieu
- cap nhat ton kho ngay sau giao dich
- co low stock de canh bao

### 5.2. Gia tri kiem soat

- tach du lieu master va giao dich
- tach quyen theo role
- ngan xoa du lieu sai
- ngan xuat vuot ton

### 5.3. Gia tri ra quyet dinh

- biet hang nao sap het
- biet gia tri ton kho dang nam trong kho
- biet mat hang nao dang van hanh
- biet luong giao dich nhap xuat

### 5.4. Gia tri mo rong

He thong hien tai la nen tang de mo rong sang:

- stock adjustment
- purchase planning
- supplier master
- customer master
- stock card
- audit trail
- reorder workflow

## 6. Pham vi du lieu cua he thong

He thong co 4 lop du lieu nghiep vu:

### 6.1. Lop phan loai

- `Category`

Tra loi:

- san pham thuoc nhom nao

### 6.2. Lop tai san hang hoa

- `Product`

Tra loi:

- kho dang quan ly mat hang nao
- ton hien tai bao nhieu
- gia von binh quan hien tai la bao nhieu

### 6.3. Lop giao dich

- `Receipt`
- `Issue`

Tra loi:

- hang da vao ra nhu the nao

### 6.4. Lop tong hop va truy vet

- `Inventory Summary`
- `Inventory Ledger`

Tra loi:

- trang thai kho hien tai
- vi sao ton kho lai thanh nhu hien tai

## 7. Dong chay thong tin trong he thong

### 7.1. Dong chay tu master den transaction

1. Admin tao category
2. Admin tao product gan vao category
3. WarehouseStaff su dung product de tao receipt va issue

Neu master data sai:

- giao dich sai theo
- summary sai theo

Nghia la:

- chat luong master data quyet dinh chat luong van hanh

### 7.2. Dong chay tu transaction den summary

1. Receipt tang ton
2. Issue giam ton
3. Product luu trang thai ton moi
4. Summary doc lai product va tinh tong hop

Nghia la:

- summary khong tu nhap tay
- summary la ket qua tu dong cua giao dich

### 7.3. Dong chay tu transaction den ledger

1. Moi line nhap kho sinh ra ledger entry
2. Moi line xuat kho sinh ra ledger entry
3. Ledger ghi lai:
   - quantity change
   - value change
   - running on hand
   - running average cost

Nghia la:

- ledger la lich su
- product la trang thai hien tai

## 8. Su khac nhau giua "trang thai hien tai" va "lich su"

Day la diem rat quan trong khi hoc business system.

### 8.1. Product la trang thai hien tai

Bang `Product` dang luu:

- con bao nhieu
- average cost hien tai
- reorder level
- co active hay khong

No tra loi cau hoi:

- "Ngay bay gio kho con gi?"

### 8.2. Receipt/Issue la giao dich

Chung tra loi:

- "Dieu gi da xay ra?"

### 8.3. Ledger la lich su truy vet

No tra loi:

- "Ton kho da thay doi the nao qua tung buoc?"

Day la 3 tang suy nghi can phan biet ro:

- state
- event
- history

## 9. Cac quyet dinh nghiep vu cot loi dang duoc he thong ap dung

### 9.1. Product la trung tam

He thong chon `Product` lam don vi trung tam.

Tat ca nhap xuat deu gan voi product.

Dieu nay dung nghiep vu vi:

- ton kho luon duoc quan ly theo SKU/mat hang

### 9.2. Average cost theo weighted average

He thong khong dung:

- FIFO
- LIFO
- standard cost

Ma dung:

- weighted average cost

Loi ich:

- de tinh
- phu hop MVP
- du de bao cao ton va gia tri ton o muc co ban

### 9.3. Product inactive khong duoc giao dich moi

Quyet dinh nay the hien nghiep vu:

- san pham ngung van hanh thi khong nen tiep tuc dua vao phieu moi

### 9.4. Khong xoa product da co giao dich

Quyet dinh nay the hien nguyen tac:

- lich su nghiep vu quan trong hon su "gon du lieu"

### 9.5. Xuat kho khong duoc am ton

Quyet dinh nay la quy tac song con cua kho:

- he thong phai ngan xuat vuot ton

## 10. Cach doc mot giao dich theo goc nhin business

### 10.1. Doc Receipt

Khi nhin 1 receipt, ban can hieu:

- day la su kien hang vao kho
- supplier la nguon vao
- line nao vao bao nhieu
- unit cost cua line do la gia nhap
- tong receipt la gia tri lo hang nhap

Y nghia tai chinh:

- tang inventory asset

Y nghia van hanh:

- tang kha nang ban/cap phat hang

### 10.2. Doc Issue

Khi nhin 1 issue, ban can hieu:

- day la su kien hang ra kho
- customer la noi nhan
- xuat bao nhieu
- line total duoc tinh theo average cost hien tai

Y nghia tai chinh:

- giam inventory asset

Y nghia van hanh:

- giam luong hang co san de tiep tuc su dung

## 11. Goc nhin quan ly kho

Neu la nguoi quan ly kho, ban khong can doc moi giao dich moi lan. Ban can nhin he thong theo 4 cau hoi:

1. Danh muc co dang sach khong
2. Ton hien tai co du khong
3. Mat hang nao sap het
4. Giao dich nao dang thay doi ton kho

He thong hien tai tra loi 4 cau hoi nay nhu sau:

- `Categories` va `Products`: danh muc co sach khong
- `Inventory Summary`: ton hien tai co du khong, hang nao sap het
- `Receipts` va `Issues`: giao dich nao dang thay doi ton kho

## 12. Goc nhin mua hang

Nguoi mua hang se quan tam:

- mat hang nao sap het
- reorder level la bao nhieu
- da tung nhap nhung lo hang nao
- average cost hien tai la bao nhieu

Du an hien tai chua co module mua hang, nhung du lieu de ra quyet dinh da co mot phan:

- low stock
- receipt history
- average cost

## 13. Goc nhin ke toan va doi soat

Nguoi ke toan se quan tam:

- gia tri ton kho hien tai
- giao dich nhap/xuat
- su thay doi gia tri ton

He thong hien tai ho tro co ban thong qua:

- `TotalInventoryValue`
- `Receipt.TotalAmount`
- `Issue.TotalAmount`
- `InventoryLedger.ValueChange`

Tuy nhien, day chua phai la module ke toan. No chi la du lieu dau vao co the phuc vu doi soat.

## 14. Goc nhin kiem soat noi bo

Kiem soat noi bo muon dam bao:

- ai duoc tao du lieu
- ai duoc sua du lieu
- ai duoc van hanh giao dich
- co xay ra xuat vuot ton khong
- co xoa du lieu quan trong khong

He thong hien tai da co:

- role-based access control
- cam xoa product co lich su
- cam xuat vuot ton
- phan tach user thao tac va user chi xem

Phan chua co:

- audit theo user tren tung giao dich
- soft delete
- log thay doi master data

## 15. Vong lap nghiep vu lap lai hang ngay

Neu van hanh thuc te, he thong nay se duoc su dung theo vong lap:

### Buoi sang

- xem dashboard
- kiem tra low stock
- kiem tra hang can xuat trong ngay

### Trong ngay

- nhap lo hang moi
- xuat hang cho don vi nhan
- theo doi ton thay doi

### Cuoi ngay

- xem lai receipt history
- xem lai issue history
- doi soat tong ton va cac giao dich bat thuong

## 16. Nhung KPI nghiep vu co the theo doi tu he thong nay

He thong hien tai chua co dashboard KPI day du, nhung da co the rut ra mot so chi so nghiep vu:

1. So san pham dang active
2. Tong don vi ton kho
3. Tong gia tri ton kho
4. So san pham low stock
5. So receipt trong ngay / tuan / thang
6. So issue trong ngay / tuan / thang
7. Gia tri nhap kho trong ky
8. Gia tri xuat kho trong ky

Khi hoc business, ban nen de y:

- KPI la "cau hoi quan ly"
- con du lieu trong bang la "nguyen lieu"

## 17. Cac rui ro kinh doanh ma he thong dang giam thieu

### 17.1. Rui ro ton kho sai

Duoc giam nho:

- cap nhat ton ngay trong transaction
- cam xuat vuot ton

### 17.2. Rui ro du lieu master bi sua/xoa linh tinh

Duoc giam nho:

- `AdminOnly` cho product/category

### 17.3. Rui ro khong biet hang sap het

Duoc giam nho:

- `ReorderLevel`
- `LowStockItems`

### 17.4. Rui ro mat lich su giao dich

Duoc giam nho:

- receipt history
- issue history
- ledger

## 18. Nhung han che kinh doanh hien tai

He thong nay da ro phan "inventory control", nhung van chua giai quyet toan bo bai toan kho:

1. Chua co kiem ke va dieu chinh ton
2. Chua co supplier/customer master
3. Chua co phe duyet giao dich
4. Chua co da kho hoac vi tri kho
5. Chua co dat hang mua
6. Chua co lien ket don ban
7. Chua co audit trail chi tiet

Vi vay, khi hoc business, ban nen hieu:

- day la he thong "core inventory"
- khong phai toan bo "warehouse management ecosystem"

## 19. Ban do nhan thuc de hoc du an nay

Neu muon hoc business qua du an nay, hay hoc theo thu tu:

1. Hieu van de ton kho la gi
2. Hieu category va product la gi
3. Hieu receipt va issue la gi
4. Hieu average cost va inventory value
5. Hieu low stock va reorder level
6. Hieu RBAC va trach nhiem van hanh
7. Hieu ledger la truy vet

Neu nam duoc 7 diem nay, ban se nhin du an nay nhu mot he thong kinh doanh, khong chi la mot app CRUD.

## 20. Ket luan

Ve mat business, he thong nay co the tom tat bang mot cau:

`MyApp Inventory la mot he thong quan ly danh muc hang hoa, ghi nhan nhap xuat, tinh ton kho hien tai, canh bao sap het hang, va kiem soat quyen thao tac theo vai tro.`

Neu rut gon hon nua:

- `Category` va `Product` tao ra bo khung hang hoa
- `Receipt` va `Issue` tao ra bien dong
- `Summary` cho biet kho dang ra sao
- `Ledger` cho biet vi sao no ra sao
- `Auth/RBAC` cho biet ai duoc phep tac dong vao tat ca nhung dieu tren

Do chinh la buc tranh tong the cua du an tu goc nhin nghiep vu.
