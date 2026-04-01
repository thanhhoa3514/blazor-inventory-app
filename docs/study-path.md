# Study Path - Lo Trinh Tu Hoc Business Tren Du An Nay

## 1. Muc dich tai lieu

Tai lieu nay bien toan bo bo docs thanh mot `lo trinh hoc co thu tu`.

Muc tieu:

- giup ban khong bi ngop khi tai lieu da nhieu
- biet nen doc gi truoc, doc gi sau
- moi giai doan hoc co muc tieu ro rang
- co bai tap tu kiem tra de biet minh da hieu den dau

Tai lieu nay duoc viet cho muc tieu:

- hoc business analysis
- hoc solution thinking
- hoc cach doc codebase business app

## 2. Ban nen hoc theo logic nao

Khi hoc mot business system, thu tu hoc tot nhat thuong la:

1. Nhin buc tranh lon
2. Hieu actor va role
3. Hieu doi tuong nghiep vu
4. Hieu giao dich
5. Hieu quy tac tinh toan
6. Hieu data model
7. Hieu API va implementation
8. Hieu roadmap va mo rong

Do la logic cua study path nay.

## 3. Giai doan 1 - Lay buc tranh tong the

### Muc tieu hoc

- biet du an nay dung de giai quyet bai toan gi
- biet he thong dang o muc inventory control hay muc cao hon
- hieu actor nao co trong he thong

### Tai lieu can doc

1. [system-overview.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/system-overview.md)
2. [business-glossary.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/business-glossary.md)

### Cau hoi tu kiem tra

1. He thong nay giai quyet van de kinh doanh nao?
2. He thong nay quan ly nhung doi tuong chinh nao?
3. Product dong vai tro gi trong he thong?
4. Inventory summary dung de lam gi?

### Dau hieu da hieu

Ban co the tu giai thich bang loi cua minh:

- he thong nay khong chi la CRUD
- no dang kiem soat ton kho, nhap xuat va summary

## 4. Giai doan 2 - Hieu vai tro va quyen

### Muc tieu hoc

- biet ai duoc lam gi
- hieu tai sao role quan trong trong kho

### Tai lieu can doc

1. [authentication-authorization-business-flow.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/authentication-authorization-business-flow.md)
2. [use-cases.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/use-cases.md)

### Cau hoi tu kiem tra

1. Tai sao Viewer duoc xem nhung khong duoc tao giao dich?
2. Tai sao WarehouseStaff duoc tao receipt/issue nhung khong duoc xoa product?
3. Tai sao Admin phai quan ly master data?

### Bai tap nho

Tu viet bang tay ma tran quyen:

- man hinh nao ai duoc vao
- action nao ai duoc lam

## 5. Giai doan 3 - Hieu luong nghiep vu kho

### Muc tieu hoc

- hieu category, product, receipt, issue, summary lien ket voi nhau the nao

### Tai lieu can doc

1. [warehouse-business-flow.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/warehouse-business-flow.md)
2. [warehouse-scenarios.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/warehouse-scenarios.md)

### Cau hoi tu kiem tra

1. Tai sao product moi tao co on hand = 0?
2. Tai sao receipt thay doi average cost?
3. Tai sao issue khong thay doi average cost?
4. Tai sao low stock la canh bao nghiep vu?

### Bai tap nho

Tu mo ta bang loi cua minh luong:

- tao product
- nhap kho
- xuat kho
- dashboard thay doi

Neu mo ta duoc luong nay khong can nhin tai lieu, ban da bat dau nho business flow.

## 6. Giai doan 4 - Hieu quy tac nghiep vu

### Muc tieu hoc

- biet he thong dang duoc dieu khien boi nhung rule nao

### Tai lieu can doc

1. [business-rules-catalog.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/business-rules-catalog.md)
2. [business-questions-checklist.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/business-questions-checklist.md)

### Cau hoi tu kiem tra

1. Rule nao la validation du lieu?
2. Rule nao la permission?
3. Rule nao la calculation?
4. Rule nao la integrity?

### Bai tap nho

Tu chon 1 feature, vi du `CreateIssue`, roi liet ke:

- 5 rule quan trong nhat

Vi du:

- phai co it nhat 1 line
- quantity > 0
- product active
- ton kho du
- actor co quyen

## 7. Giai doan 5 - Hieu du lieu va domain model

### Muc tieu hoc

- noi business voi entity, request, dto
- hieu tai sao mo hinh du lieu duoc thiet ke nhu vay

### Tai lieu can doc

1. [domain-model-explained.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/domain-model-explained.md)
2. [erd-business-explained.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/erd-business-explained.md)

### Cau hoi tu kiem tra

1. Tai sao product la tam cua ERD?
2. Tai sao receipt/issue can line table?
3. Tai sao ledger can luu running values?
4. Khac nhau giua entity va DTO?

### Bai tap nho

Tu ve ERD bang tay voi cac box:

- Category
- Product
- Receipt
- ReceiptLine
- Issue
- IssueLine
- Ledger
- User/Role

Sau do tu giai thich tung moi quan he bang loi business.

## 8. Giai doan 6 - Hieu API va implementation map business the nao

### Muc tieu hoc

- biet business di vao he thong qua API ra sao
- biet controller/service dai dien cho buoc nao trong use case

### Tai lieu can doc

1. [api-business-mapping.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/api-business-mapping.md)
2. [event-storming-notes.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/event-storming-notes.md)

### Cau hoi tu kiem tra

1. API nao tao receipt?
2. Business event nao xay ra sau create issue?
3. Policy nao dang chan Viewer tao issue?
4. Event nao lam thay doi on hand?

### Bai tap nho

Chon 1 use case `CreateReceipt`, roi tu map:

- actor
- command
- policy
- event
- state change
- API
- UI

Neu map duoc 7 muc nay, ban dang hieu he thong o muc rat tot.

## 9. Giai doan 7 - Hieu tuong lai va hoc cach tu phan tich roadmap

### Muc tieu hoc

- biet he thong se lon len theo huong nao
- hieu tai sao mot tinh nang duoc uu tien

### Tai lieu can doc

1. [future-roadmap.md](/home/thanhhoa/thanhhoa/blazor-inventory-app/docs/future-roadmap.md)

### Cau hoi tu kiem tra

1. Tai sao stock adjustment la uu tien cao?
2. Tai sao supplier/customer master nen lam som?
3. Tai sao audit trail quan trong ve business?
4. Tai sao reorder workflow la buoc chuyen tu visibility sang planning?

### Bai tap nho

Tu viet them 1 sprint moi theo format:

- business goal
- user stories
- acceptance criteria

Vi du:

- sprint "stock transfer"
- sprint "multi warehouse"

## 10. Lo trinh 7 ngay de hoc nhanh

### Ngay 1

- Doc `system-overview.md`
- Doc `business-glossary.md`

Muc tieu:

- nhin duoc buc tranh tong

### Ngay 2

- Doc `authentication-authorization-business-flow.md`
- Doc `use-cases.md`

Muc tieu:

- hieu actor va permission

### Ngay 3

- Doc `warehouse-business-flow.md`
- Doc `warehouse-scenarios.md`

Muc tieu:

- hieu quy trinh nhap xuat va van hanh thuc te

### Ngay 4

- Doc `business-rules-catalog.md`
- Doc `business-questions-checklist.md`

Muc tieu:

- hoc cach nhin business qua rule va cau hoi

### Ngay 5

- Doc `domain-model-explained.md`
- Doc `erd-business-explained.md`

Muc tieu:

- noi business sang data model

### Ngay 6

- Doc `api-business-mapping.md`
- Doc `event-storming-notes.md`

Muc tieu:

- noi business sang implementation

### Ngay 7

- Doc `future-roadmap.md`
- Tu tom tat lai toan he thong bang ngon ngu cua ban

Muc tieu:

- biet phan tich hien tai va tuong lai cua he thong

## 11. Lo trinh 14 ngay de hoc sau hon

Neu muon hoc sau hon, ban co the chia nho moi ngay chi hoc 1 tai lieu va co bai tap.

### Tuan 1

Ngay 1:

- `system-overview.md`

Ngay 2:

- `business-glossary.md`

Ngay 3:

- `authentication-authorization-business-flow.md`

Ngay 4:

- `warehouse-business-flow.md`

Ngay 5:

- `warehouse-scenarios.md`

Ngay 6:

- `use-cases.md`

Ngay 7:

- tu tom tat 6 tai lieu dau

### Tuan 2

Ngay 8:

- `business-rules-catalog.md`

Ngay 9:

- `business-questions-checklist.md`

Ngay 10:

- `domain-model-explained.md`

Ngay 11:

- `erd-business-explained.md`

Ngay 12:

- `api-business-mapping.md`

Ngay 13:

- `event-storming-notes.md`

Ngay 14:

- `future-roadmap.md`
- tu viet lai "story cua he thong" bang loi cua minh

## 12. Bai tap tu danh gia nang luc

Sau khi hoc xong, hay tu thu suc voi 10 bai tap sau:

1. Giai thich tai sao receipt thay doi average cost bang loi cua ban.
2. Giai thich tai sao issue khong thay doi average cost.
3. Giai thich tai sao product la tam cua domain model.
4. Giai thich tai sao category khong duoc xoa khi con product.
5. Giai thich tai sao viewer khong duoc tao issue.
6. Tu viet use case cho `StockAdjustment`.
7. Tu viet 5 business rules cho `StockAdjustment`.
8. Tu ve event storming cho `StockAdjustment`.
9. Tu viet 1 sprint roadmap cho `Supplier master`.
10. Tu phan biet:
   - entity
   - request
   - dto
   tren mot vi du cu the

Neu lam duoc 10 bai nay, ban da nam du an o muc kha manh.

## 13. Cach biet ban dang tien bo dung huong

Ban dang tien bo dung huong khi:

1. Ban nhin man hinh va tu hoi:
   - actor nao dung man nay
   - man nay phuc vu quyet dinh gi
2. Ban nhin API va tu hoi:
   - day la use case nao
   - event nao xay ra sau no
3. Ban nhin table va tu hoi:
   - doi tuong nghiep vu nao dang duoc model hoa
4. Ban nhin backlog va tu hoi:
   - van de business nao dang duoc giai quyet

Neu ban dat duoc nhung cau hoi nay tu nhien, ban dang hoc business rat dung cach.

## 14. Sai lam pho bien khi hoc business system

### Sai lam 1 - Chi hoc UI

Ban se biet click o dau, nhung khong biet tai sao.

### Sai lam 2 - Chi hoc code

Ban se biet method nao chay, nhung khong biet quy tac nghiep vu phia sau.

### Sai lam 3 - Chi hoc database

Ban se biet quan he bang, nhung khong biet actor va use case.

### Sai lam 4 - Doc tai lieu ma khong tu tom tat lai

Ban se cam giac "da doc", nhung thuc ra chua noi hoa thanh kien thuc cua minh.

### Sai lam 5 - Khong tu dat cau hoi

Business analysis la ky nang dat cau hoi.  
Neu khong hoi, ban kho ma hieu sau.

## 15. Cong thuc hoc nhanh nhat

Cong thuc hoc nhanh nhat tren bo docs nay la:

1. Doc 1 tai lieu
2. Tu tom tat bang 5-10 dong
3. Tu dat 3 cau hoi business moi
4. Noi tai lieu do voi code hoac UI
5. Tu mo ta lai bang ngon ngu cua minh

Lam lap lai, ban se nho rat lau.

## 16. Study outcome mong muon

Sau khi di het lo trinh nay, ban nen dat duoc 5 nang luc:

1. Hieu business flow cua du an
2. Hieu domain model va ERD cua du an
3. Hieu permission va role model
4. Hieu cach business map vao API va service
5. Tu viet duoc backlog va business rule cho feature moi

## 17. Ket luan

Study path nay duoc tao de bien bo docs thanh mot chuong trinh hoc thuc te.

Muc tieu cuoi cung khong phai la "doc het file".  
Muc tieu cuoi cung la:

- ban co the tu giai thich du an nay nhu mot he thong nghiep vu
- ban co the tu phan tich feature moi bang ngon ngu business
- va ban co the noi business voi code mot cach mach lac
