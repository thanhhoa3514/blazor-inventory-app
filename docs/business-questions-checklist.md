# Business Questions Checklist - Checklist Cau Hoi Khi Phan Tich He Thong Quan Ly Kho

## 1. Muc dich tai lieu

Tai lieu nay la mot `checklist tu duy business` de ban dung khi:

- hoc mot he thong kho moi
- phan tich yeu cau tu user
- review mot du an inventory
- tu dat cau hoi de hieu business truoc khi hieu code

Day khong phai tai lieu mo ta "he thong nay da co gi".  
Day la tai lieu mo ta:

- can phai hoi nhung gi
- hoi de lam ro van de kinh doanh nao
- cau hoi nao dan toi thiet ke nghiep vu dung

Noi cach khac:

- day la cong cu hoc business
- khong chi la cong cu ghi chep

## 2. Cach dung checklist nay

Ban co the dung tai lieu nay theo 3 cach:

### Cach 1 - Hoc du an hien tai

Doc tung nhom cau hoi va tu tra loi dua tren du an `MyApp Inventory`.

### Cach 2 - Di phong van stakeholder

Khi gap:

- chu doanh nghiep
- quan ly kho
- thu kho
- ke toan
- bo phan mua hang

Ban co the dung cac cau hoi nay de hoi cho dung trong tam.

### Cach 3 - Tu review mot he thong da ton tai

Neu nhin vao codebase ma thay kho hieu, hay dung checklist de tu hoi:

- he thong nay dang co gia thuyet nghiep vu nao
- quy tac nao dang duoc ngam dinh ma chua viet ra

## 3. Nguyen tac dat cau hoi business

Khi dat cau hoi business, co 5 nguyen tac:

1. Hoi ve `muc tieu`, khong chi hoi ve `man hinh`.
2. Hoi ve `quy trinh thuc te`, khong chi hoi ve `du lieu`.
3. Hoi ve `ngoai le`, khong chi hoi ve happy path.
4. Hoi ve `quyen va trach nhiem`, khong chi hoi ve chuc nang.
5. Hoi ve `quyet dinh duoc dua ra tu du lieu nao`.

Neu ban chi hoi:

- co man hinh nao
- co form nao
- co table nao

thi ban moi chi hoc UI, chua hoc business.

## 4. Nhom cau hoi ve bai toan kinh doanh tong the

### 4.1. He thong nay giai quyet van de kinh doanh nao

Muc dich:

- xac dinh he thong duoc sinh ra de giai quyet pain point gi

Cau hoi:

1. Doanh nghiep dang gap van de gi trong viec quan ly kho?
2. Neu khong co he thong nay thi cong viec hien tai duoc lam bang cach nao?
3. Dieu gi dang ton kem nhat:
   - thoi gian
   - sai sot
   - ton kho
   - that thoat
   - kho ra quyet dinh
4. Muc tieu cua he thong la:
   - kiem soat ton
   - giam sai sot
   - ho tro mua hang
   - ho tro bao cao
   - hay tat ca?

### 4.2. He thong nay dang o muc do nao

Muc dich:

- xac dinh pham vi business that su

Cau hoi:

1. Day la he thong chi de theo doi ton kho hay la he thong kho day du?
2. Co bao gom mua hang, ban hang, kiem ke, approval khong?
3. Day la he thong cho 1 kho hay nhieu kho?
4. He thong nay tac nghiep hay da ho tro planning?

## 5. Nhom cau hoi ve actor va vai tro

### 5.1. Ai su dung he thong

Cau hoi:

1. Ai la nguoi dang nhap he thong?
2. Moi vai tro can thay gi?
3. Moi vai tro can thao tac gi?
4. Vai tro nao chi xem?
5. Vai tro nao duoc thay doi du lieu?

### 5.2. Ai chiu trach nhiem cho du lieu

Cau hoi:

1. Ai tao product?
2. Ai tao category?
3. Ai tao receipt?
4. Ai tao issue?
5. Ai duyet thao tac nhay cam?
6. Neu sai ton kho, doanh nghiep muon truy ve ai?

### 5.3. Quyen va nghiep vu co trung nhau khong

Cau hoi:

1. Nguoi co quyen tao giao dich co phai cung la nguoi giao hang/nhan hang khong?
2. Nguoi duyet co khac nguoi tao khong?
3. Nguoi xem bao cao co can thay thong tin nhay cam khong?

## 6. Nhom cau hoi ve du lieu master

### 6.1. Ve Category

Cau hoi:

1. Tai sao can category?
2. Category dung de phan loai theo logic nao?
3. Category co duoc doi ten khong?
4. Co duoc xoa category khong?
5. Dieu gi xay ra neu category da co product?

### 6.2. Ve Product

Cau hoi:

1. Product la gi trong nghiep vu cua doanh nghiep nay?
2. SKU do ai dat?
3. SKU co bat buoc unique khong?
4. Product inactive co duoc xuat hien trong giao dich khong?
5. Product co duoc xoa khong?
6. Khi nao product duoc xem la ngung van hanh?

### 6.3. Ve master data rong hon

Cau hoi:

1. Supplier co la master data khong?
2. Customer co la master data khong?
3. Don vi tinh co can quan ly khong?
4. Co batch, serial, expiry hay khong?
5. Co vi tri kho/bin location khong?

## 7. Nhom cau hoi ve ton kho hien tai

### 7.1. Ton kho la gi trong he thong nay

Cau hoi:

1. "Ton kho" dang duoc hieu la gi?
2. Co phan biet:
   - on hand
   - available
   - reserved
   - damaged
   - in transit
   hay khong?
3. He thong co cho phep ton am khong?
4. Neu ton am la khong hop le, he thong chan o dau?

### 7.2. Ton kho duoc cap nhat khi nao

Cau hoi:

1. Khi tao receipt co tang ton ngay khong?
2. Khi tao issue co giam ton ngay khong?
3. Co ton tai trang thai draft/approved truoc khi tac dong ton khong?
4. Co tinh huong nao ton kho thay doi ma khong qua receipt/issue khong?

## 8. Nhom cau hoi ve nghiep vu nhap kho

### 8.1. Nhap kho dung trong nhung truong hop nao

Cau hoi:

1. Nhap kho la mua hang, nhan tra hang, hay bo sung noi bo?
2. Co nhieu loai receipt khong?
3. Nguoi dung co can chon supplier khong?
4. Co can ghi note khong?

### 8.2. Cac quy tac khi nhap kho

Cau hoi:

1. Receipt co bat buoc co it nhat 1 line khong?
2. Product phai active moi duoc nhap hay khong?
3. Unit cost co bat buoc nhap khong?
4. Unit cost co the bang 0 khong?
5. Receipt co duoc sua sau khi tao khong?
6. Receipt co duoc xoa khong?

### 8.3. Vanh de business sau nhap kho

Cau hoi:

1. Sau receipt co can sinh so chung tu khong?
2. Co can in phieu khong?
3. Co can doi chieu voi supplier khong?
4. Co can approval cho receipt lon khong?

## 9. Nhom cau hoi ve nghiep vu xuat kho

### 9.1. Xuat kho dung trong nhung truong hop nao

Cau hoi:

1. Xuat kho la ban hang, cap phat noi bo, hay giao cho don vi nao?
2. Co can ghi customer/noi nhan khong?
3. Co nhieu loai issue khong?

### 9.2. Cac quy tac khi xuat kho

Cau hoi:

1. Co duoc xuat vuot ton khong?
2. Xuat theo gia nao:
   - average cost
   - FIFO
   - gia ban
   - standard cost
3. Issue co duoc sua sau khi tao khong?
4. Issue co duoc huy khong?
5. Co can approval neu issue gia tri lon khong?

### 9.3. Van de business sau xuat kho

Cau hoi:

1. Sau issue co giam ton ngay khong?
2. Co can in phieu xuat khong?
3. Co can doi chieu voi bo phan nhan hang khong?

## 10. Nhom cau hoi ve dieu chinh ton kho

Muc dich:

- lam ro he thong co can `Stock Adjustment` hay khong

Cau hoi:

1. Khi kiem ke lech ton thi doanh nghiep xu ly the nao?
2. Co truong hop hao hut, hu hong, mat mat khong?
3. Co can giao dich dieu chinh ton rieng khong?
4. Adjustment co can ly do bat buoc khong?
5. Adjustment co can approval khong?
6. Ai duoc phep adjustment?

Neu cau tra loi la "co", thi adjustment la nghiep vu cot loi, khong phai tinh nang phu.

## 11. Nhom cau hoi ve gia tri ton kho va cost

### 11.1. He thong tinh gia von theo cach nao

Cau hoi:

1. Doanh nghiep dung:
   - weighted average
   - FIFO
   - LIFO
   - standard cost
2. Gia nhap moi co anh huong average cost khong?
3. Xuat kho co lam thay doi average cost khong?

### 11.2. Gia tri ton kho duoc su dung de lam gi

Cau hoi:

1. Co can hien total inventory value khong?
2. Con so nay dung cho:
   - quan tri
   - doi soat
   - ke toan
   - planning
3. Co can bao cao theo ky khong?

## 12. Nhom cau hoi ve canh bao va quyet dinh

### 12.1. Low stock

Cau hoi:

1. San pham nao duoc xem la sap het?
2. Cong thuc la:
   - nho hon reorder level
   - bang reorder level
   - hay theo quy tac khac?
3. Ai can nhin canh bao nay?
4. Sau khi canh bao, ai hanh dong?

### 12.2. Reorder planning

Cau hoi:

1. He thong chi canh bao hay can de xuat so luong nhap?
2. Co preferred stock level khong?
3. Co lead time cua supplier khong?
4. Co uu tien san pham quan trong hon khong?

## 13. Nhom cau hoi ve lich su va truy vet

### 13.1. Can truy vet den muc nao

Cau hoi:

1. Doanh nghiep co can stock card khong?
2. Co can biet vi sao ton kho thay doi khong?
3. Co can xem receipt/issue history theo product khong?
4. Co can luu running on hand va running average cost khong?

### 13.2. Can truy vet actor khong

Cau hoi:

1. Ai tao receipt nay?
2. Ai tao issue nay?
3. Ai sua product nay?
4. Co can audit trail khong?

Neu stakeholder rat quan tam den "ai lam", thi audit la bat buoc.

## 14. Nhom cau hoi ve quyen va kiem soat noi bo

### 14.1. Permission

Cau hoi:

1. Ai duoc tao master data?
2. Ai duoc tao giao dich?
3. Ai duoc xoa du lieu?
4. Viewer co duoc xem het du lieu hay chi mot phan?

### 14.2. Approval

Cau hoi:

1. Co thao tac nao can phe duyet khong?
2. Nguoi tao co duoc tu duyet khong?
3. Muc gia tri nao can approval?
4. Adjustment co can approval khong?

### 14.3. Separation of duties

Cau hoi:

1. Nguoi tao co duoc xoa giao dich cua minh khong?
2. Nguoi quan tri master data co phai la nguoi van hanh kho khong?
3. Co can tach role kiem soat va role tac nghiep khong?

## 15. Nhom cau hoi ve bao cao

### 15.1. Bao cao nao quan trong

Cau hoi:

1. Stakeholder can nhin bao cao nao nhieu nhat?
2. Can bao cao:
   - ton hien tai
   - low stock
   - nhap trong ky
   - xuat trong ky
   - gia tri ton
   - stock card
   hay gi khac?

### 15.2. Muc do chi tiet cua bao cao

Cau hoi:

1. Chi can summary hay can detail?
2. Can loc theo:
   - ngay
   - category
   - SKU
   - supplier
   - customer
3. Can export excel/csv khong?

## 16. Nhom cau hoi ve lifecycle cua du lieu

### 16.1. Xoa hay vo hieu hoa

Cau hoi:

1. Category co duoc xoa that khong?
2. Product co nen xoa that hay inactive?
3. Giao dich co duoc xoa sau khi tao khong?
4. Co can soft delete khong?

### 16.2. Sua du lieu sau khi phat sinh giao dich

Cau hoi:

1. Product da co giao dich co duoc doi category khong?
2. Product da co giao dich co duoc doi SKU khong?
3. Receipt/issue da tao co duoc sua khong?

## 17. Nhom cau hoi ve ngoai le va tinh huong kho

### 17.1. Ngoai le nghiep vu

Cau hoi:

1. Neu nhap nham product thi lam sao?
2. Neu xuat nham so luong thi lam sao?
3. Neu phat hien ton lech thi lam sao?
4. Neu supplier/customer bi nhap sai ten thi lam sao?

### 17.2. Ngoai le van hanh

Cau hoi:

1. Neu 2 nguoi cung thao tac cung luc thi sao?
2. Co can chong double-submit khong?
3. Co can idempotency cho giao dich nhap/xuat khong?

## 18. Nhom cau hoi ve mo rong tuong lai

### 18.1. He thong co se lon len theo huong nao

Cau hoi:

1. Sau inventory control, buoc tiep theo la gi?
2. Can stock adjustment khong?
3. Can supplier/customer master khong?
4. Can reorder workflow khong?
5. Can approval khong?
6. Can multi-warehouse khong?

### 18.2. Dau hieu cua feature can co som

Cau hoi:

1. User co lien tuc hoi "vi sao ton lech" khong?
2. User co lien tuc hoi "hang nao sap het" khong?
3. User co lien tuc nhap supplier/customer bang tay roi bi sai khong?
4. User co can doi soat ai tao giao dich khong?

Neu co, do la dau hieu roadmap can uu tien.

## 19. Nhom cau hoi de tu hoc business tren du an nay

Ban co the tu luyen bang cach tu tra loi cac cau hoi sau dua tren code va docs hien tai:

1. He thong nay la inventory control hay warehouse management day du?
2. Tai sao product la trung tam cua mo hinh?
3. Tai sao receipt thay doi average cost nhung issue thi khong?
4. Tai sao viewer duoc xem nhung khong duoc tao giao dich?
5. Tai sao khong duoc xoa product da co giao dich?
6. Tai sao low stock la canh bao business, khong phai loi he thong?
7. He thong dang thieu nghiep vu nao de dung duoc ngoai doi thuc te hon?

Neu ban tra loi ro 7 cau nay, kha nang hieu business cua ban se len rat nhanh.

## 20. Checklist rut gon de mang di phan tich

Neu can mot ban rut gon, day la 15 cau hoi quan trong nhat:

1. He thong giai quyet van de kinh doanh nao?
2. Ai su dung he thong?
3. Ai duoc xem, ai duoc thao tac?
4. Du lieu master la gi?
5. SKU duoc quan ly the nao?
6. Ton kho duoc dinh nghia ra sao?
7. Nhap kho xay ra trong truong hop nao?
8. Xuat kho xay ra trong truong hop nao?
9. Co duoc xuat vuot ton khong?
10. Gia von duoc tinh theo cach nao?
11. San pham nao duoc xem la low stock?
12. Co can adjustment khong?
13. Co can audit ai tao giao dich khong?
14. Bao cao nao la quan trong nhat?
15. He thong se mo rong theo huong nao tiep theo?

## 21. Cach biet ban da bat dau hieu business that

Ban bat dau hieu business that khi ban khong chi hoi:

- "truong nay la gi?"

ma bat dau hoi:

- "truong nay phuc vu quyet dinh kinh doanh nao?"

Ban khong chi hoi:

- "tai sao can API nay?"

ma hoi:

- "API nay dang hien thuc use case nao cua actor nao?"

Ban khong chi hoi:

- "tai sao can bang ledger?"

ma hoi:

- "neu khong co ledger thi doanh nghiep se mat kha nang nao?"

Do la dau hieu rat tot.

## 22. Ket luan

Checklist nay la cong cu de ban tu luyen tu duy business analysis.

No giup ban:

- dat cau hoi dung
- thay duoc logic nghiep vu an duoi code
- hieu tai sao he thong duoc thiet ke nhu vay

Trong he thong business, chat luong cau hoi thuong quan trong hon chat luong cau tra loi ban dau.  
Hoi dung, ban se dan den thiet ke dung.  
Hoi sai, ban co the code rat dep nhung van giai quyet sai bai toan.
