CREATE TABLE [dbo].[Cities]
(
	[CityId] INT PRIMARY KEY IDENTITY(1, 1),
	[Name] NVARCHAR(64) NOT NULL,
)
GO

CREATE TABLE [dbo].[Universities]
(
	[UniversityId] INT PRIMARY KEY IDENTITY(1, 1),
	[Name] NVARCHAR(128) NOT NULL,
)
GO

CREATE TABLE [dbo].[Faculties]
(
	[FacultyId] INT PRIMARY KEY IDENTITY(1, 1),
	[Name] NVARCHAR(128) NOT NULL,

	[UniversityId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Universities]([UniversityId]),
)
GO

CREATE TABLE [dbo].[Roles]
(
	[RoleId] INT PRIMARY KEY IDENTITY(1, 1),
	[Name] NVARCHAR(32) NOT NULL
)
GO

CREATE TABLE [dbo].[UserPermissions]
(
	[UserPermissionId] INT PRIMARY KEY IDENTITY(1, 1),
	[AllowToWriteOnMyWall] BIT NOT NULL,
	[AllowToWriteComments] BIT NOT NULL,
	[AllowToSendMeAMessage] BIT NOT NULL
)
GO

CREATE TABLE [dbo].[StudentDetails]
(
	[StudentDetailsId] INT PRIMARY KEY IDENTITY(1, 1),
	[UniversityId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Universities]([UniversityId]),
	[FacultyId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Faculties]([FacultyId]),
	[Course] INT NOT NULL
)
GO

CREATE TABLE [dbo].[Users]
(
	[UserId] INT PRIMARY KEY IDENTITY(1, 1),
	[Username] NVARCHAR(64) NOT NULL,
	[Password] NVARCHAR(64) NOT NULL,
	[Email] NVARCHAR(64) NOT NULL,
	[Status] NVARCHAR(1024) NOT NULL,
	[Avatar] NVARCHAR(512) NOT NULL,
	[Fullname] NVARCHAR(128) NOT NULL,
	[Rating] FLOAT NOT NULL,
	
	[DateOfBirth] DATETIME2 NOT NULL,
	[NotificationsLastChecked] DATETIME2 NOT NULL,
	
	[StudentDetailsId] INT FOREIGN KEY REFERENCES [dbo].[StudentDetails]([StudentDetailsId]),
	[RoleId] INT FOREIGN KEY REFERENCES [dbo].[StudentDetails]([StudentDetailsId]),
	[CityId] INT FOREIGN KEY REFERENCES [dbo].[Cities]([CityId]),
)
GO

CREATE TABLE [dbo].[UserRecoveries]
(
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[HashString] VARCHAR(128) NOT NULL
)
GO

CREATE TABLE [dbo].[Friends]
(
	[UserId1] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[UserId2] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
)
GO

CREATE TABLE [dbo].[Subscribers]
(
	[UserId1] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[UserId2] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
)
GO

CREATE TABLE [dbo].[BlackList]
(
	[UserId1] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[UserId2] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
)
GO

CREATE TABLE [dbo].[WallPosts]
(
	[WallPostId] INT PRIMARY KEY IDENTITY(1, 1),
	[Content] NVARCHAR(MAX) NOT NULL,
	
	[OwnerId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[SenderId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
)
GO

CREATE TABLE [dbo].[WallPostLikes]
(
	[LikerId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[WallPostId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[WallPosts]([WallPostId])
)
GO

CREATE TABLE [dbo].[WallPostComments]
(
	[WallPostCommentId] INT PRIMARY KEY IDENTITY(1, 1),
	[Content] NVARCHAR(MAX) NOT NULL,
	[CreationTime] DATETIME2 NOT NULL,

	[CreatorId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[WallPostId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[WallPosts]([WallPostId])
)
GO

CREATE TABLE [dbo].[Files]
(
	[FileId] INT PRIMARY KEY IDENTITY(1, 1),
	[FileName] NVARCHAR(64) NOT NULL,
	[RealFileName] NVARCHAR(128) NOT NULL
)
GO

CREATE TABLE [dbo].[WallPostFiles]
(
	[WallPostId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[WallPosts]([WallPostId]),
	[FileId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Files]([FileId]),
)
GO

CREATE TABLE [dbo].[Messages]
(
	[MessageId] INT PRIMARY KEY IDENTITY(1, 1),
	[Content] NVARCHAR(MAX) NOT NULL,
	[CreationDate] DATETIME2 NOT NULL,

	[SenderId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[ReceiverId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
)
GO

CREATE TABLE [dbo].[MessageFiles]
(
	[MessageId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Messages]([MessageId]),
	[FileId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Files]([FileId])
)
GO

CREATE TABLE [dbo].[Notifications]
(
	[NotificationId] INT PRIMARY KEY IDENTITY(1, 1),
	[Message] NVARCHAR(MAX) NOT NULL,
	[Link] NVARCHAR(128) NOT NULL,
	[Type] INT NOT NULL,
	[Date] DATETIME2 NOT NULL,
	[Seen] BIT NOT NULL,
	[SeenDate] DATETIME2,

	[SenderId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[ReceiverId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
)
GO

CREATE TABLE [dbo].[TaskCategories]
(
	[TaskCategoryId] INT PRIMARY KEY IDENTITY(1, 1),
	[Name] NVARCHAR(128) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL
)
GO

CREATE TABLE [dbo].[TaskSubcategories]
(
	[TaskSubcategoryId] INT PRIMARY KEY IDENTITY(1, 1),
	[Name] NVARCHAR(128) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,

	[TaskCategoryId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[TaskCategories]([TaskCategoryId])
)
GO

CREATE TABLE [dbo].[Tasks]
(
	[TaskId] INT PRIMARY KEY IDENTITY(1, 1),
	[Header] NVARCHAR(512) NOT NULL,
	[Content] NVARCHAR(MAX) NOT NULL,
	[Price] MONEY NOT NULL,
	[Solved] BIT,
	[Hide] BIT,

	[CreatorId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[ExecutiveId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[TaskSubcategoryId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[TaskSubcategories]([TaskSubcategoryId])
)
GO

CREATE TABLE [dbo].[TaskSubscribers]
(
	[TaskId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Tasks]([TaskId]),
	[SubscriberId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[Price] MONEY NOT NULL,
)
GO

CREATE TABLE [dbo].[TaskComments]
(
	[TaskCommentId] INT PRIMARY KEY IDENTITY(1, 1),
	[Content] NVARCHAR(MAX) NOT NULL,
	[CreationTime] DATETIME2 NOT NULL,

	[CreatorId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[TaskId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Tasks]([TaskId])
)
GO

CREATE TABLE [dbo].[TaskLikes]
(
	[LikerId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[TaskId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Tasks]([TaskId])
)
GO

CREATE TABLE [dbo].[TaskExecutionReviews]
(
	[TaskExecutionReviewId] INT PRIMARY KEY IDENTITY(1, 1),
	[Review] NVARCHAR(MAX) NOT NULL,
	[Rating] FLOAT NOT NULL,

	[TaskId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Tasks]([TaskId]),
	[ExecutorId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
)
GO

CREATE TABLE [dbo].[TaskFiles]
(
	[TaskId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Tasks]([TaskId]),
	[FileId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Files]([FileId]),
)
GO

CREATE TABLE [dbo].[Achievements]
(
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Users]([UserId]),
	[TaskCategoryId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[TaskCategories]([TaskCategoryId])
)
GO