# Warehouse Scenarios - Tinh Huong Van Hanh Thuc Te Trong Kho

## 1. Muc dich tai lieu

Tai lieu nay mo ta cac tinh huong van hanh thuc te theo kieu case study.

Muc tieu:

- giup ban thay business khong chi la schema va API
- giup hieu nguoi dung su dung he thong trong doi song van hanh ra sao
- ket noi ly thuyet voi luong nghiep vu thuc te

Tai lieu nay rat quan trong neu ban muon "cam" du an nhu mot he thong kinh doanh, khong phai mot bai tap CRUD.

## 2. Cach doc scenario

Moi scenario gom:

1. `Boi canh`
2. `Actor`
3. `Van de business`
4. `Cach he thong duoc su dung`
5. `Du lieu nao thay doi`
6. `Gia tri business dat duoc`
7. `Rui ro neu khong co he thong`

## 3. Scenario 1 - Mot ngay van hanh kho thong thuong

### Boi canh

Kho dang van hanh hang ngay voi nhieu mat hang thong dung.

### Actor

- WarehouseStaff
- Viewer
- Admin

### Dien bien

#### Buoc 1 - Dau ngay

Viewer hoac WarehouseStaff vao dashboard de xem:

- tong ton kho
- so mat hang dang active
- danh sach low stock

#### Buoc 2 - Kiem tra can nhap

Neu co san pham low stock:

- quan ly co the danh dau can theo doi
- thu kho biet san pham nao can de y

#### Buoc 3 - Nhan hang ve

Kho nhan duoc mot lo hang tu nha cung cap.

WarehouseStaff:

- vao man `Receipts`
- nhap supplier
- them cac line san pham
- nhap quantity va unit cost
- submit phieu

#### Buoc 4 - He thong cap nhat

Sau khi submit receipt:

- ton kho tang
- average cost duoc tinh lai
- receipt history co them mot phieu moi
- ledger co them cac dong bien dong

#### Buoc 5 - Xuat hang

Trong ngay, kho can cap phat hoac xuat hang.

WarehouseStaff:

- vao man `Issues`
- nhap customer/noi nhan
- chon san pham va so luong
- submit phieu

#### Buoc 6 - He thong cap nhat tiep

Sau issue:

- ton kho giam
- average cost giu nguyen
- issue history duoc cap nhat
- summary thay doi theo

#### Buoc 7 - Cuoi ngay

Nguoi quan ly co the:

- xem receipt history
- xem issue history
- xem dashboard moi nhat

### Gia tri business

- du lieu ton kho cap nhat ngay
- nhap xuat co dau vet
- co the doi soat cuoi ngay

### Rui ro neu khong co he thong

- de ghi sai tay
- khong biet ton thuc te
- kho doi soat ai lam gi

## 4. Scenario 2 - Admin mo khoi dong he thong moi

### Boi canh

Doanh nghiep bat dau dua mot nhom hang moi vao quan ly.

### Actor

- Admin

### Van de business

Truoc khi nhap xuat duoc, he thong phai co danh muc chuan.

### Cach he thong duoc su dung

1. Admin tao cac `Category`
2. Admin tao cac `Product`
3. Dat `ReorderLevel` cho tung product
4. Product duoc xem la san sang cho van hanh

### Du lieu thay doi

- `Categories`
- `Products`

### Gia tri business

- tao bo khung danh muc chuan
- tranh thu kho nhap ten hang tuy y

### Bai hoc business

Van hanh kho tot luon bat dau tu master data tot.

## 5. Scenario 3 - Kho nhap mot lo hang voi gia moi

### Boi canh

San pham da co ton truoc do, nhung lo nhap moi co gia khac.

### Actor

- WarehouseStaff

### Van de business

Can cap nhat ton kho va gia von binh quan dung theo lo nhap moi.

### Vi du

Truoc khi nhap:

- Wireless Mouse con 20
- AverageCost = 10
- Tong gia tri ton = 200

Nay nhap them:

- 10 cai
- UnitCost = 12

### Cach he thong xu ly

1. Tao receipt line quantity = 10, unit cost = 12
2. He thong tinh:
   - gia tri nhap = 120
   - ton moi = 30
   - gia tri moi = 320
   - average cost moi = 10.67 neu tinh day du, he thong se lam tron 2 chu so theo quy tac hien tai

### Gia tri business

- bao cao ton kho phan anh sat hon gia tri thuc te

### Bai hoc business

Kho khong chi quan ly "so luong".  
Kho con quan ly "gia tri ton".

## 6. Scenario 4 - Thu kho co gang xuat vuot ton

### Boi canh

Kho chi con 3 san pham, nhung thu kho vo tinh xuat 5.

### Actor

- WarehouseStaff

### Van de business

Neu he thong khong chan, ton kho se am va bao cao sai.

### Cach he thong xu ly

1. User tao issue quantity = 5
2. Server kiem tra `OnHandQty`
3. Phat hien ton hien tai chi co 3
4. Tu choi giao dich
5. Bao loi ro rang

### Gia tri business

- bao ve su dung dan cua ton kho
- tranh he thong "song ao"

### Bai hoc business

Trong kho, mot quy tac co ban nhung song con la:

- khong du hang thi khong duoc xuat

## 7. Scenario 5 - Quan ly muon biet mat hang nao sap het

### Boi canh

Nguoi quan ly khong co thoi gian xem tung product.

### Actor

- Viewer
- Manager

### Van de business

Can mot man tong hop de uu tien hanh dong.

### Cach he thong duoc su dung

1. Mo dashboard
2. Xem:
   - `LowStockCount`
   - `LowStockItems`
3. Nhin SKU, ten san pham, ton hien tai, reorder level

### Gia tri business

- ra quyet dinh nhanh
- tranh het hang

### Bai hoc business

Dashboard tot khong phai chi de "dep".  
No phai giam thoi gian ra quyet dinh.

## 8. Scenario 6 - Admin muon xoa mot product cu

### Boi canh

Mot product da ngung kinh doanh. Admin muon xoa khoi he thong.

### Actor

- Admin

### Co 2 tinh huong

#### Truong hop A - Chua co giao dich

He thong cho xoa.

#### Truong hop B - Da co receipt hoac issue

He thong khong cho xoa.

### Tai sao business can nhu vay

Neu xoa product da co lich su:

- cac phieu cu mat lien ket
- bao cao cu mat nghia
- truy vet sai lech khong con chinh xac

### Bai hoc business

He thong kinh doanh thuong uu tien:

- giu lich su

hon la:

- gon danh muc bang moi gia

## 9. Scenario 7 - Viewer vao he thong nhung khong duoc tao giao dich

### Boi canh

Quan ly chi muon xem bao cao, khong muon tham gia van hanh.

### Actor

- Viewer

### Cach he thong xu ly

1. Viewer dang nhap thanh cong
2. Duoc vao dashboard, receipts, issues, products, categories
3. Khong thay form tao giao dich
4. Neu goi API tao giao dich truc tiep:
   - server tra `403 Forbidden`

### Gia tri business

- tach ro vai tro xem va vai tro thao tac
- giam rui ro can thiep nham

### Bai hoc business

Phan quyen dung la phan quyen o ca:

- UI
- API

## 10. Scenario 8 - Receipt bi nhap sai product inactive

### Boi canh

Mot product da duoc de `inactive`, nhung nhan vien vo tinh chon lai no.

### Actor

- WarehouseStaff

### Cach he thong xu ly

1. UI chi co xu huong cho chon product active
2. Neu van co request len server voi product inactive
3. Service kiem tra va tu choi

### Gia tri business

- dam bao business rule khong chi nam o giao dien

### Bai hoc business

Trong he thong nghiep vu:

- UI co the giup nguoi dung
- nhung server moi la noi bao ve quy tac

## 11. Scenario 9 - Doi soat receipt cu

### Boi canh

Quan ly can xem lai mot phieu nhap cu de doi chieu nha cung cap.

### Actor

- Viewer
- WarehouseStaff
- Admin

### Cach he thong duoc su dung

1. Vao `/receipts`
2. Tim den phieu can xem
3. Mo detail
4. Xem:
   - document no
   - supplier
   - note
   - tung line
   - unit cost
   - total amount

### Gia tri business

- phuc vu kiem tra lai giao dich
- de doi chieu chung tu

## 12. Scenario 10 - Kho sap het nhung chua co reorder workflow

### Boi canh

San pham da vao low stock, nhung he thong hien chua co de xuat dat hang.

### Actor

- Viewer
- Manager

### Thuc te business hien tai

He thong moi cho biet:

- hang nao sap het

Chu chua cho biet:

- can nhap bao nhieu
- nhap tu supplier nao
- uu tien hang nao truoc

### Bai hoc business

Day la su khac nhau giua:

- inventory visibility

va

- inventory planning

He thong hien tai dang manh o visibility, chua di sau vao planning.

## 13. Scenario 11 - Kiem ke phat hien lech ton

### Boi canh

Kho dem thuc te thay con 18 nhung he thong dang bao 20.

### Van de business

He thong hien tai chua co `Stock Adjustment`.

### Neu van hanh voi phien ban hien tai

Nguoi dung co the bi cam do:

- khong co nghiep vu dung de xu ly chenh lech

### Bai hoc business

Mot he thong kho day du phai co:

- giao dich van hanh thong thuong
- giao dich dieu chinh ngoai le

Day la ly do `Stock Adjustment` la backlog quan trong.

## 14. Scenario 12 - Ong chu doanh nghiep muon hoi "Kho dang nam bao nhieu tien?"

### Boi canh

Chu doanh nghiep khong can chi tiet tung line, ma can mot con so tong quan.

### Actor

- Admin
- Viewer

### Cach he thong tra loi

Dashboard tra ve:

- `TotalInventoryValue`

### Y nghia business

Con so nay giup nguoi quan ly hieu:

- bao nhieu gia tri dang nam trong kho
- ton kho dang nang ve tai chinh ra sao

### Bai hoc business

Kho khong chi la van de logistics.  
Kho con la van de dong von.

## 15. Scenario 13 - Nhan vien moi vao hoc su dung he thong

### Boi canh

Co nhan vien moi vao kho va can hoc quy trinh.

### Cach huan luyen bang he thong

1. Giai thich `Product` la gi
2. Giai thich `Receipt` la gi
3. Giai thich `Issue` la gi
4. Giai thich ton kho va average cost
5. Giai thich tai sao khong duoc xuat vuot ton
6. Giai thich tai sao co role

### Gia tri business

He thong co quy tac ro se giup onboard nhan vien nhanh hon.

### Bai hoc business

Mot he thong tot la mot phan cua tai lieu huan luyen van hanh.

## 16. Scenario 14 - Su co sai master data

### Boi canh

Admin tao nham product vao sai category.

### Anh huong business

- bao cao theo category sai
- kho tim kiem sai nhom
- low stock theo nhom bi vo nghia

### Cach he thong xu ly

Admin co the sua product va doi category.

### Bai hoc business

Master data sai khong lam hong giao dich ngay lap tuc, nhung se lam xau he thong ve lau dai.

## 17. Scenario 15 - Nhu cau tim "vi sao ton kho thay doi"

### Boi canh

Quan ly thay ton kho Wireless Mouse giam manh, can biet ly do.

### He thong hien tai lam duoc gi

Co the:

- xem issue history
- xem receipt history

### He thong hien tai chua lam duoc gi tot nhat

Chua co man:

- stock card cho tung product

### Bai hoc business

Khi he thong lon len, cau hoi se khong con la:

- con bao nhieu

ma la:

- tai sao lai con bao nhieu

## 18. Scenario 16 - Cuoi thang doi soat voi ke toan

### Boi canh

Cuoi thang, can doi chieu tong gia tri nhap, xuat, ton.

### He thong hien tai ho tro mot phan

- receipt total amount
- issue total amount
- inventory summary total inventory value
- ledger value change

### Khoang trong

- chua co report/export chuan
- chua co stock adjustment
- chua co audit theo user

### Bai hoc business

He thong hien tai tot cho tac nghiep va tong hop co ban, nhung muon doi soat manh hon thi can roadmap mo rong.

## 19. Cach tu dat scenario de hoc business

Khi hoc business, ban co the tu dat cau hoi theo mau:

1. Nguoi nao dang gap van de gi
2. Ho can ra quyet dinh gi
3. He thong cung cap thong tin nao de ho ra quyet dinh
4. Neu thong tin sai thi anh huong business la gi

Vi du:

- Neu low stock sai thi dieu gi xay ra
- Neu receipt sai unit cost thi total inventory value sai ra sao
- Neu issue khong bi chan khi vuot ton thi he thong vo o diem nao

## 20. Ket luan

Case study la cach hoc business rat tot vi no giup ban thay:

- du lieu song nhu the nao trong van hanh
- quy tac nghiep vu co gia tri gi
- tai sao he thong phai duoc thiet ke nhu hien tai

Neu tai lieu flow cho ban "logic",  
thi scenario cho ban "cam giac van hanh that".
