# Future Roadmap - Backlog Nghiep Vu Va Ky Thuat Theo Tung Sprint

## 1. Muc dich tai lieu

Tai lieu nay bien danh sach y tuong mo rong thanh mot `roadmap co cau truc`.

Muc tieu:

- giup ban thay ro he thong kho thuong phat trien theo thu tu nao
- hieu tai sao moi tinh nang duoc uu tien
- thay moi sprint khong chi la code, ma la giai quyet mot van de kinh doanh cu the
- hoc cach chuyen tu "y tuong" sang "backlog co nghiep vu, acceptance criteria va rui ro"

Tai lieu nay viet theo huong:

- business first
- product second
- technical implementation third

## 2. Nguyen tac lap roadmap cho he thong kho

Khi xay he thong kho, khong nen nhay vao tinh nang phuc tap qua som. Thu tu hop ly thuong la:

1. Co du lieu master dung
2. Co giao dich nhap xuat dung
3. Co phan quyen dung
4. Co truy vet dung
5. Co canh bao dung
6. Co quy trinh nang cao sau

He thong hien tai da co:

- category
- product
- receipt
- issue
- summary
- auth / RBAC
- ledger

Vi vay roadmap tiep theo nen uu tien:

1. truy vet user thao tac
2. stock adjustment
3. supplier/customer master
4. stock card va search/filter
5. soft delete va audit trail
6. reorder workflow

## 3. Cach doc backlog trong tai lieu nay

Moi sprint se co:

1. `Business goal`
2. `Van de hien tai`
3. `Pham vi sprint`
4. `User stories`
5. `Acceptance criteria`
6. `Technical scope`
7. `Risk and note`
8. `Gia tri hoc business`

## 4. Sprint 1 - Hardening Auth Va Gan User Vao Giao Dich

### Business goal

Tu he thong "co phan quyen" tien len he thong "co trach nhiem nguoi thao tac".

### Van de hien tai

He thong da biet role nao duoc lam gi, nhung chua tra loi duoc:

- ai tao receipt nay
- ai tao issue nay
- ai sua product nay
- ai xoa category nay

Trong business thuc te, day la lo hong lon.

### Pham vi sprint

1. Gan nguoi thuc hien vao receipt
2. Gan nguoi thuc hien vao issue
3. Gan nguoi tao/cap nhat vao product va category neu phu hop
4. Bo sung log hoac audit co ban
5. Bao mat hon cho seed account dev/test

### User stories

#### Story 1

Voi tu cach la `Warehouse Manager`, toi muon biet ai tao mot receipt, de co the truy trach nhiem khi doi soat kho.

#### Story 2

Voi tu cach la `Admin`, toi muon biet ai tao issue, de biet ai da thuc hien xuat hang.

#### Story 3

Voi tu cach la `Security owner`, toi muon bo seed password khoi file config production, de giam rui ro ro ri thong tin.

### Acceptance criteria

1. Moi receipt moi phai luu `CreatedByUserId` hoac thong tin user tuong duong.
2. Moi issue moi phai luu `CreatedByUserId`.
3. UI chi tiet receipt/issue hien duoc nguoi tao.
4. Test xac nhan user dang login tao giao dich nao thi giao dich do ghi dung user.
5. Password dev/test co the seed trong env hoac user-secrets, khong de o config production.

### Technical scope

1. Mo rong entity `StockReceipt`, `StockIssue`.
2. Cap nhat service tao giao dich de lay current user.
3. Them abstraction lay user hien tai.
4. Cap nhat DTO detail.
5. Cap nhat UI detail.
6. Cap nhat integration tests.

### Risk and note

- Can quyet dinh luu `UserId`, `UserName`, hay ca hai.
- Neu ve sau doi username, luu `UserId` se an toan hon.

### Gia tri hoc business

Sprint nay day ban hoc mot nguyen ly rat quan trong:

- business system khong chi ghi "su kien"
- business system phai ghi "chu the gay ra su kien"

## 5. Sprint 2 - Stock Adjustment

### Business goal

Bo sung nghiep vu dieu chinh ton kho de xu ly chenh lech thuc te.

### Van de hien tai

He thong hien chi co:

- nhap kho
- xuat kho

Nhung kho thuc te luon phat sinh:

- hao hut
- hu hong
- mat mat
- lech kiem ke
- phat hien ton thua

Neu khong co adjustment:

- nguoi dung se phai "fake" bang receipt hoac issue
- du lieu nghiep vu se sai y nghia

### Pham vi sprint

1. Them `StockAdjustment`
2. Cho phep dieu chinh tang hoac giam ton
3. Bat buoc nhap ly do dieu chinh
4. Ghi ledger ro movement type la `ADJUSTMENT`

### User stories

#### Story 1

Voi tu cach la `WarehouseStaff`, toi muon ghi nhan chenh lech sau kiem ke, de ton kho he thong khop voi ton thuc te.

#### Story 2

Voi tu cach la `Manager`, toi muon biet ly do dieu chinh ton, de tranh viec thay doi ton kho tuy tien.

### Acceptance criteria

1. Co man tao adjustment.
2. Moi adjustment phai chon huong:
   - tang ton
   - giam ton
3. Moi adjustment phai co reason code hoac free-text reason.
4. Giam ton khong duoc lam ton am neu business yeu cau.
5. Ledger phai the hien adjustment la movement rieng.

### Technical scope

1. Entity `StockAdjustment`, `StockAdjustmentLine`
2. API CRUD can thiet cho create/get
3. Validation cho increase/decrease
4. UI adjustment page
5. Summary va ledger lien thong

### Risk and note

- Can quyet dinh adjustment duoc phep cho role nao:
  - chi Admin
  - hay Admin + InventoryManager

### Gia tri hoc business

Sprint nay day ban hoc su khac nhau giua:

- giao dich kinh doanh thong thuong
- giao dich dieu chinh he thong

## 6. Sprint 3 - Supplier Va Customer Master

### Business goal

Chuyen `Supplier` va `Customer` tu text tu do thanh du lieu master co quan ly.

### Van de hien tai

Receipt va Issue hien tai dung:

- `Supplier: string?`
- `Customer: string?`

Rui ro business:

- cung 1 supplier nhung nhap nhieu kieu ten
- kho thong ke theo doi tac
- kho doi chieu lich su

### Pham vi sprint

1. Tao master `Supplier`
2. Tao master `Customer`
3. Cho receipt tham chieu supplier
4. Cho issue tham chieu customer

### User stories

#### Story 1

Voi tu cach la `Purchasing`, toi muon chon supplier tu danh muc co san, de tranh nhap sai ten nha cung cap.

#### Story 2

Voi tu cach la `Manager`, toi muon thong ke giao dich theo tung supplier va customer.

### Acceptance criteria

1. Co man CRUD supplier/customer.
2. Receipt co the chon supplier tu combobox.
3. Issue co the chon customer tu combobox.
4. Du lieu cu co chien luoc migration hop ly.
5. Bao cao theo supplier/customer khong bi duplicate ten.

### Technical scope

1. Entity supplier/customer
2. Migration FK
3. Update DTO va UI
4. Searchable select
5. Backfill du lieu text cu neu can

### Risk and note

- Can quyet dinh giu text tu do song song hay bat buoc dung master 100%

### Gia tri hoc business

Sprint nay day ban hoc bai hoc:

- text field thuong giai quyet nhanh
- nhung master data moi giai quyet duoc tinh quan tri ve lau dai

## 7. Sprint 4 - Stock Card Va Transaction Search

### Business goal

Tang kha nang tra cuu va truy vet cho quan ly va doi soat.

### Van de hien tai

He thong co ledger nhung chua co:

- man stock card theo product
- bo loc transaction tot
- tim nhanh theo SKU, ngay, doc no

### Pham vi sprint

1. Man `Stock Card` theo product
2. Search/filter cho receipt va issue
3. Loc theo ngay, SKU, category, document no
4. Xem lich su bien dong theo thoi gian

### User stories

#### Story 1

Voi tu cach la `Warehouse Manager`, toi muon xem toan bo bien dong cua mot product, de truy vet vi sao ton thay doi.

#### Story 2

Voi tu cach la `Auditor`, toi muon loc giao dich theo ngay va document no, de doi soat nhanh.

### Acceptance criteria

1. Co trang stock card cho product.
2. Hien duoc:
   - ngay giao dich
   - movement type
   - reference no
   - quantity change
   - running on hand
   - running average cost
3. Receipt va issue list co filter theo ngay va tu khoa.
4. Hieu nang tra cuu van chap nhan duoc.

### Technical scope

1. Query API moi cho ledger
2. Them index neu can
3. UI filter/search nang cao
4. Product detail link den stock card

### Risk and note

- Khi du lieu lon dan, can chu y phan trang va query projection

### Gia tri hoc business

Sprint nay day ban hoc:

- quan ly kho khong chi can "state hien tai"
- ma can "explainability" cua state do

## 8. Sprint 5 - Soft Delete Va Audit Trail

### Business goal

Tang kha nang kiem soat noi bo, tranh mat du lieu quan trong, va dam bao moi thay doi deu co dau vet.

### Van de hien tai

Du an hien co mot so thao tac xoa that.

Trong moi truong business that, xoa that co the:

- gay mat du lieu lich su
- kho doi soat
- kho phuc hoi

### Pham vi sprint

1. Soft delete cho category/product/master data
2. Audit trail cho thao tac:
   - create
   - update
   - delete
3. Ghi ai, luc nao, thay doi gi

### User stories

#### Story 1

Voi tu cach la `Admin`, toi muon ngung su dung mot product ma khong mat du lieu lich su.

#### Story 2

Voi tu cach la `Auditor`, toi muon xem lich su thay doi du lieu master, de truy vet sai sot hoac thay doi trai phep.

### Acceptance criteria

1. Xoa category/product se thanh soft delete neu nghiep vu phu hop.
2. UI mac dinh an ban ghi da xoa mem.
3. Co the xem lich su thay doi co ban.
4. Audit log ghi:
   - actor
   - action
   - timestamp
   - target entity

### Technical scope

1. Them field soft delete
2. Global query filter
3. Audit table
4. Middleware/service phat hien thay doi
5. UI xem log co ban

### Risk and note

- Can thong nhat "inactive" va "soft deleted" khac nhau the nao

### Gia tri hoc business

Sprint nay day ban phan biet:

- du lieu khong con van hanh
- va du lieu khong con ton tai

Trong business, hai khai niem nay khong giong nhau.

## 9. Sprint 6 - Reorder Workflow

### Business goal

Chuyen low stock tu "canh bao bi dong" thanh "goi y hanh dong".

### Van de hien tai

He thong da biet hang sap het, nhung chua biet:

- can nhap bao nhieu
- uu tien hang nao truoc
- nha cung cap nao phu hop

### Pham vi sprint

1. Bo sung `PreferredStockLevel`
2. De xuat so luong can nhap
3. Danh sach reorder candidates
4. Tao draft purchase request

### User stories

#### Story 1

Voi tu cach la `Purchasing`, toi muon he thong goi y san pham can nhap va so luong de xuat, de ra quyet dinh nhanh hon.

#### Story 2

Voi tu cach la `Manager`, toi muon uu tien nhung san pham low stock quan trong truoc.

### Acceptance criteria

1. Co danh sach reorder recommendation.
2. Moi product low stock hien:
   - on hand
   - reorder level
   - suggested reorder quantity
3. Co quy tac tinh suggested quantity ro rang.
4. Co the export hoac tao draft de xu ly tiep.

### Technical scope

1. Them field planning
2. Query recommendation
3. UI reorder dashboard
4. Potential draft entity ve sau

### Risk and note

- Cong thuc reorder can ro rang, neu khong se mat y nghia nghiep vu

### Gia tri hoc business

Sprint nay day ban chuyen tu:

- inventory visibility

sang:

- inventory planning

## 10. Sprint 7 - Approval Workflow Cho Giao Dich Nhay Cam

### Business goal

Kiem soat giao dich lon hoac dieu chinh ton bang co che phe duyet.

### Van de hien tai

He thong hien la:

- co quyen la thao tac duoc

Nhung o doanh nghiep lon hon:

- co quyen chua du
- con can luong phe duyet

### Pham vi sprint

1. Giao dich co trang thai:
   - draft
   - pending approval
   - approved
   - rejected
2. Adjustment lon can phe duyet
3. Issue gia tri lon can phe duyet

### User stories

#### Story 1

Voi tu cach la `Warehouse Staff`, toi muon gui issue gia tri lon cho quan ly phe duyet, de tuan thu quy trinh.

#### Story 2

Voi tu cach la `Manager`, toi muon duyet hoac tu choi giao dich nhay cam, de kiem soat rui ro.

### Acceptance criteria

1. Giao dich co workflow status.
2. Nguoi tao giao dich khong tu duyet giao dich cua chinh minh neu policy can tach.
3. Giao dich chi tac dong ton kho khi dat trang thai duoc phep theo quy tac nghiep vu da chot.

### Technical scope

1. State machine cho transaction
2. Policy approval
3. UI inbox pending approvals
4. Notification co ban

### Gia tri hoc business

Sprint nay day ban:

- business process khong phai luc nao cung la "click xong ngay"
- nhieu he thong thuc te phai co approval gate

## 11. Sprint 8 - Bao Cao Va Export

### Business goal

Bien he thong tu noi ghi nhan du lieu thanh noi phuc vu ra quyet dinh va doi soat ngoai he thong.

### Van de hien tai

Du lieu co san nhung chua de:

- xuat excel
- in phieu
- chia se bao cao

### Pham vi sprint

1. Export receipt/issue history
2. Export stock summary
3. In phieu nhap / phieu xuat
4. Bao cao ton kho theo thoi diem

### User stories

#### Story 1

Voi tu cach la `Warehouse Staff`, toi muon in phieu nhap/xuat, de doi chieu voi quy trinh giay to thuc te.

#### Story 2

Voi tu cach la `Manager`, toi muon export summary va history, de tong hop va chia se cho cac bo phan.

### Acceptance criteria

1. Xuat file csv/xlsx cho lich su giao dich.
2. In duoc receipt/issue voi dinh dang de doc.
3. Bao cao ton kho theo bo loc co the export.

### Technical scope

1. Export service
2. Print-friendly layout
3. Report queries

### Gia tri hoc business

Sprint nay day ban:

- gia tri cua he thong khong dung lai o man hinh
- ma o kha nang dua du lieu ra phuc vu quy trinh thuc te

## 12. Epic map tong hop

### Epic A - Trust and control

Bao gom:

- auth hardening
- audit trail
- approval workflow

Muc tieu:

- he thong dang tin
- he thong co trach nhiem

### Epic B - Inventory truth

Bao gom:

- stock adjustment
- stock card
- search and traceability

Muc tieu:

- so lieu kho phan anh sat thuc te

### Epic C - Business master data

Bao gom:

- supplier master
- customer master

Muc tieu:

- du lieu van hanh khong bi roi rac

### Epic D - Planning and reporting

Bao gom:

- reorder workflow
- export/reporting

Muc tieu:

- tu he thong tac nghiep tien len he thong ho tro quyet dinh

## 13. Thu tu uu tien de trien khai thuc te

Neu chi co it nguon luc, thu tu thuc dung nhat la:

1. Sprint 1 - hardening auth va gan user vao giao dich
2. Sprint 2 - stock adjustment
3. Sprint 4 - stock card va search
4. Sprint 3 - supplier/customer master
5. Sprint 5 - audit trail va soft delete
6. Sprint 6 - reorder workflow
7. Sprint 8 - reporting/export
8. Sprint 7 - approval workflow

Ly do:

- truoc phai dam bao giao dich co chu the va truy vet duoc
- sau do phai dam bao ton kho co the dieu chinh dung
- sau do moi nang kha nang tra cuu, quyet dinh, va quy trinh nang cao

## 14. Bieu mau user story de hoc business

Khi tu viet backlog, ban co the dung mau nay:

`Voi tu cach la [vai tro], toi muon [hanh dong], de [gia tri kinh doanh].`

Vi du:

- Voi tu cach la Warehouse Staff, toi muon tao stock adjustment, de chinh ton kho he thong sau kiem ke.
- Voi tu cach la Manager, toi muon xem stock card cua tung product, de truy vet vi sao ton kho thay doi.
- Voi tu cach la Purchasing, toi muon nhan danh sach reorder recommendation, de lap ke hoach mua hang.

## 15. Bieu mau acceptance criteria de hoc business

Ban co the viet acceptance criteria theo mau:

1. Dieu kien ban dau
2. Hanh dong nguoi dung
3. Ket qua he thong
4. Rang buoc nghiep vu

Vi du:

1. Cho san pham A co ton = 5
2. Khi nhan vien tao issue quantity = 7
3. Thi he thong tu choi giao dich
4. Va hien thong bao khong du ton

Do la cach bien logic business thanh tieu chi nghiem thu.

## 16. Dieu quan trong nhat de hoc business tu roadmap nay

Moi sprint tot khong phai la sprint co nhieu ticket nhat.  
Moi sprint tot la sprint tra loi duoc cau hoi:

- Van de kinh doanh nao dang duoc giai quyet
- Neu khong lam sprint nay thi he thong se vuong cho nao
- Sau sprint nay nguoi dung van hanh duoc tot hon o diem nao

Neu ban luon dat 3 cau hoi nay, ban dang hoc business dung cach.

## 17. Ket luan

Roadmap nay cho thay he thong kho thuong lon len theo cac buoc:

1. Ghi nhan du lieu
2. Kiem soat du lieu
3. Truy vet du lieu
4. Quan tri du lieu
5. Ra quyet dinh tu du lieu

Du an hien tai dang o giai doan:

- da ghi nhan du lieu
- da kiem soat quyen
- dang san sang de nang cap truy vet va planning

Do la diem rat tot de hoc business system vi ban co the nhin ro cach mot MVP inventory app tien hoa thanh mot he thong kho truong thanh hon.
