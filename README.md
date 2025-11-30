# HMGame_Test

### 1. 📂 `HMGame_Test_Part1` Logic Test
Thư mục này chứa **lời giải cho 3 bài test logic** Part 1 của đề bài
- **Aim:** Tập trung vào thuật toán và tư duy giải quyết vấn đề bằng C#, bao gồm các script xử lý các yêu cầu cụ thể của bài toán
- **Comment code:** Hiểu flow, giải thích và cho ví dụ để dễ hiểu cách hoạt động của từng hàm.
- **Link Video:** https://youtu.be/c-cEkl1MJ4w  (Nếu link video xem không được vui lòng mail giúp em hongocthaigamedev@gmail.com)
- **How to use:** Mở file solution trong thư mục lớn, máy bạn có visual studio thì sẽ tự động mở dự án lên cho anh chị tesrt.

### 2. 📂 `Demo Test Match 3 Fish_Part2` New Gameplay 
Đây là Unity Project chứa **Source code gốc đã được thay đổi Gameplay**
- **Mechanic:** Match-3 (Xếp hình).
- **Mode:** 4 mode: chơi thường, chơi autoplay win, chơi auto play thua,mode tính thời gian và có thể reverse tấm tile về vị trí cũ
- **Feature:** Cập nhật logic game, điều kiện thắng/thua và các tính năng gameplay mới dựa trên yêu cầu của Part 2.
- **Description:** Gồm có các script core bao gồm BoardGenerator, AutoplayHandler, GameManager, TileManager trong folder Script xử lý các nhiệm vụ core theo yêu cầu bài toán đưa ra.
- BoardGenerator: hàm core CreateSpriteBag() đóng vai trò sinh bộ 3, sau đó dùng thuật toán tráo bài shuffle để trộn vị trí, đảm bảo game có lời giải, GenerateBoard() để sinh lớp layer chơi, ResolveLayerDimensions() điều chỉnh kích thước layer.
- AutoplayHandler : chịu trách nhiệm quản lý coroutine tự động chơi.
- GameManager : lớp quản lý trạng thái game, mode game.
- TileManager : quản lý các layer, các tile, trạng thái win, lose, hàm quan trọng UnlockNextLayer() sau khi chơi xong layer hiện tại thì mở khóa layer dưới
- **How to use:** Mở folder này bằng Unity Hub, vào home scene để khởi tạo.
- **BUG:** Đôi khi sẽ có bài bug nhỏ, vì đây là prototype nên em chưa kịp fix.

### 3. 📂 `Reskin_Part2` Reskin Version
Đây là phiên bản Unity Project đã được **Reskin (Thay đổi giao diện)**.
- Thay đổi Sprite
---
