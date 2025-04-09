CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,  -- Đây chính là UserID
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    DisplayName NVARCHAR(100),
    IsOnline BIT DEFAULT 0,
    AvatarLink NVARCHAR(255)
);


CREATE TABLE Messages (
    ID INT PRIMARY KEY IDENTITY,
    SenderID INT NOT NULL,
    ReceiverID INT NOT NULL,
    Content NVARCHAR(MAX),
    Time DATETIME DEFAULT GETDATE(),
    MessageType NVARCHAR(20) DEFAULT 'text',
    IsDelete BIT DEFAULT 0,
    FOREIGN KEY (SenderID) REFERENCES Users(UserId),
    FOREIGN KEY (ReceiverID) REFERENCES Users(UserId)
);

CREATE TABLE Friends (
    ID INT PRIMARY KEY IDENTITY,
    UserID INT NOT NULL,
    FriendID INT NOT NULL,
    Status NVARCHAR(20) DEFAULT 'pending',
    FOREIGN KEY (UserID) REFERENCES Users(UserId),
    FOREIGN KEY (FriendID) REFERENCES Users(UserId)
);

CREATE TABLE CallHistory (
    ID INT PRIMARY KEY IDENTITY,
    CallerID INT NOT NULL,
    ReceiverID INT NOT NULL,
    CallTime DATETIME DEFAULT GETDATE(),
    Duration INT,
    CallType NVARCHAR(10) DEFAULT 'audio',
    FOREIGN KEY (CallerID) REFERENCES Users(UserId),
    FOREIGN KEY (ReceiverID) REFERENCES Users(UserId)
);

CREATE TABLE Groups (
    ID INT PRIMARY KEY IDENTITY,
    GroupName NVARCHAR(100) NOT NULL,
    CreateBy INT NOT NULL,
    AvatarPath NVARCHAR(255),
    FOREIGN KEY (CreateBy) REFERENCES Users(UserId)
);

CREATE TABLE GroupMembers (
    ID INT PRIMARY KEY IDENTITY,
    GroupID INT NOT NULL,
    UserID INT NOT NULL,
    IsAdmin BIT DEFAULT 0,
    FOREIGN KEY (GroupID) REFERENCES Groups(ID),
    FOREIGN KEY (UserID) REFERENCES Users(UserId)
);

CREATE TABLE GroupMessages (
    ID INT PRIMARY KEY IDENTITY,
    GroupID INT NOT NULL,
    SenderID INT NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    MessageType NVARCHAR(50) DEFAULT 'text',
    IsDelete BIT DEFAULT 0,
    FOREIGN KEY (GroupID) REFERENCES Groups(ID),
    FOREIGN KEY (SenderID) REFERENCES Users(UserId)
);
