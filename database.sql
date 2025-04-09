-- Bảng lưu thông tin người dùng
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,                  -- Khóa chính, tự động tăng
    Username NVARCHAR(50) NOT NULL UNIQUE,            -- Tên đăng nhập, không trùng lặp
    Password NVARCHAR(100) NOT NULL,                  -- Mật khẩu (đã mã hóa)
    DisplayName NVARCHAR(100),                        -- Tên hiển thị của người dùng
    IsOnline BIT DEFAULT 0,                           -- Trạng thái online (0: offline, 1: online)
    AvatarLink NVARCHAR(255)                          -- Đường dẫn đến ảnh đại diện
);

-- Bảng lưu tin nhắn giữa 2 người dùng
CREATE TABLE Messages (
    ID INT PRIMARY KEY IDENTITY,                      -- Khóa chính, tự động tăng
    SenderID INT NOT NULL,                            -- ID người gửi
    ReceiverID INT NOT NULL,                          -- ID người nhận
    Content NVARCHAR(MAX),                            -- Nội dung tin nhắn
    Time DATETIME DEFAULT GETDATE(),                  -- Thời gian gửi, mặc định là thời điểm hiện tại
    MessageType NVARCHAR(20) DEFAULT 'text',          -- Loại tin nhắn (text, image, video, v.v.)
    IsDelete BIT DEFAULT 0,                           -- Trạng thái xóa (0: chưa xóa, 1: đã xóa)
    FOREIGN KEY (SenderID) REFERENCES Users(UserId),  -- Khóa ngoại liên kết với người gửi
    FOREIGN KEY (ReceiverID) REFERENCES Users(UserId) -- Khóa ngoại liên kết với người nhận
);

-- Bảng lưu danh sách bạn bè
CREATE TABLE Friends (
    ID INT PRIMARY KEY IDENTITY,                      -- Khóa chính, tự động tăng
    UserID INT NOT NULL,                              -- ID người dùng
    FriendID INT NOT NULL,                            -- ID bạn bè
    Status NVARCHAR(20) DEFAULT 'pending',            -- Trạng thái kết bạn (pending, accepted, blocked, ...)
    FOREIGN KEY (UserID) REFERENCES Users(UserId),
    FOREIGN KEY (FriendID) REFERENCES Users(UserId)
);

-- Bảng lưu lịch sử cuộc gọi giữa 2 người dùng
CREATE TABLE CallHistory (
    ID INT PRIMARY KEY IDENTITY,                      -- Khóa chính
    CallerID INT NOT NULL,                            -- ID người gọi
    ReceiverID INT NOT NULL,                          -- ID người nhận cuộc gọi
    CallTime DATETIME DEFAULT GETDATE(),              -- Thời gian bắt đầu cuộc gọi
    Duration INT,                                     -- Thời lượng cuộc gọi (giây)
    CallType NVARCHAR(10) DEFAULT 'audio',            -- Loại cuộc gọi (audio, video)
    FOREIGN KEY (CallerID) REFERENCES Users(UserId),
    FOREIGN KEY (ReceiverID) REFERENCES Users(UserId)
);

-- Bảng lưu thông tin các nhóm chat
CREATE TABLE Groups (
    ID INT PRIMARY KEY IDENTITY,                      -- Khóa chính
    GroupName NVARCHAR(100) NOT NULL,                 -- Tên nhóm
    CreateBy INT NOT NULL,                            -- Người tạo nhóm
    AvatarPath NVARCHAR(255),                         -- Ảnh đại diện nhóm
    FOREIGN KEY (CreateBy) REFERENCES Users(UserId)
);

-- Bảng lưu thành viên trong nhóm
CREATE TABLE GroupMembers (
    ID INT PRIMARY KEY IDENTITY,                      -- Khóa chính
    GroupID INT NOT NULL,                             -- ID nhóm
    UserID INT NOT NULL,                              -- ID người dùng
    IsAdmin BIT DEFAULT 0,                            -- Là admin nhóm hay không (1: có, 0: không)
    FOREIGN KEY (GroupID) REFERENCES Groups(ID),
    FOREIGN KEY (UserID) REFERENCES Users(UserId)
);

-- Bảng lưu tin nhắn trong nhóm
CREATE TABLE GroupMessages (
    ID INT PRIMARY KEY IDENTITY,                      -- Khóa chính
    GroupID INT NOT NULL,                             -- ID nhóm
    SenderID INT NOT NULL,                            -- ID người gửi
    Content NVARCHAR(MAX) NOT NULL,                   -- Nội dung tin nhắn
    Timestamp DATETIME DEFAULT GETDATE(),             -- Thời gian gửi
    MessageType NVARCHAR(50) DEFAULT 'text',          -- Loại tin nhắn (text, image, ...)
    IsDelete BIT DEFAULT 0,                           -- Trạng thái xóa
    FOREIGN KEY (GroupID) REFERENCES Groups(ID),
    FOREIGN KEY (SenderID) REFERENCES Users(UserId)
);
