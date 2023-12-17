
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/17/2023 23:42:51
-- Generated from EDMX file: D:\Dev\HotelManagement\HotelManagement\HotelManagement\Models\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [HotelManagement];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__FOLIO__room_book__3B75D760]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FOLIO] DROP CONSTRAINT [FK__FOLIO__room_book__3B75D760];
GO
IF OBJECT_ID(N'[dbo].[FK__FOLIO__service_i__3C69FB99]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FOLIO] DROP CONSTRAINT [FK__FOLIO__service_i__3C69FB99];
GO
IF OBJECT_ID(N'[dbo].[FK__GUEST_BOO__guest__35BCFE0A]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GUEST_BOOKING] DROP CONSTRAINT [FK__GUEST_BOO__guest__35BCFE0A];
GO
IF OBJECT_ID(N'[dbo].[FK__GUEST_BOO__reser__34C8D9D1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GUEST_BOOKING] DROP CONSTRAINT [FK__GUEST_BOO__reser__34C8D9D1];
GO
IF OBJECT_ID(N'[dbo].[FK__GUEST_BOO__room___36B12243]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GUEST_BOOKING] DROP CONSTRAINT [FK__GUEST_BOO__room___36B12243];
GO
IF OBJECT_ID(N'[dbo].[FK__INVOICE__reserva__3F466844]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[INVOICE] DROP CONSTRAINT [FK__INVOICE__reserva__3F466844];
GO
IF OBJECT_ID(N'[dbo].[FK__RESERVATI__main___2E1BDC42]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RESERVATION] DROP CONSTRAINT [FK__RESERVATI__main___2E1BDC42];
GO
IF OBJECT_ID(N'[dbo].[FK__ROOM__roomtype_i__2B3F6F97]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ROOM] DROP CONSTRAINT [FK__ROOM__roomtype_i__2B3F6F97];
GO
IF OBJECT_ID(N'[dbo].[FK__ROOM_BOOK__reser__30F848ED]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ROOM_BOOKED] DROP CONSTRAINT [FK__ROOM_BOOK__reser__30F848ED];
GO
IF OBJECT_ID(N'[dbo].[FK__ROOM_BOOK__room___31EC6D26]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ROOM_BOOKED] DROP CONSTRAINT [FK__ROOM_BOOK__room___31EC6D26];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ACCOUNT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACCOUNT];
GO
IF OBJECT_ID(N'[dbo].[CHARGES]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CHARGES];
GO
IF OBJECT_ID(N'[dbo].[FOLIO]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FOLIO];
GO
IF OBJECT_ID(N'[dbo].[GUEST]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GUEST];
GO
IF OBJECT_ID(N'[dbo].[GUEST_BOOKING]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GUEST_BOOKING];
GO
IF OBJECT_ID(N'[dbo].[INVOICE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[INVOICE];
GO
IF OBJECT_ID(N'[dbo].[RESERVATION]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RESERVATION];
GO
IF OBJECT_ID(N'[dbo].[ROOM]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ROOM];
GO
IF OBJECT_ID(N'[dbo].[ROOM_BOOKED]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ROOM_BOOKED];
GO
IF OBJECT_ID(N'[dbo].[ROOMTYPE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ROOMTYPE];
GO
IF OBJECT_ID(N'[dbo].[SERVICE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SERVICE];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ACCOUNTs'
CREATE TABLE [dbo].[ACCOUNTs] (
    [id] int IDENTITY(1,1) NOT NULL,
    [username] varchar(50)  NOT NULL,
    [password] varchar(100)  NOT NULL,
    [permission] varchar(20)  NULL
);
GO

-- Creating table 'CHARGES'
CREATE TABLE [dbo].[CHARGES] (
    [id] int  NOT NULL,
    [surcharge] float  NULL,
    [over_capacity_fee] float  NULL,
    [early_checkin_fee] float  NULL,
    [late_checkout_fee] float  NULL
);
GO

-- Creating table 'FOLIOs'
CREATE TABLE [dbo].[FOLIOs] (
    [id] int IDENTITY(1,1) NOT NULL,
    [room_booked_id] int  NOT NULL,
    [service_id] int  NOT NULL,
    [amount] int  NULL
);
GO

-- Creating table 'GUESTs'
CREATE TABLE [dbo].[GUESTs] (
    [id] varchar(12)  NOT NULL,
    [name] nvarchar(50)  NULL,
    [gender] varchar(20)  NULL,
    [birthday] datetime  NULL,
    [address] nvarchar(100)  NULL,
    [phone] varchar(10)  NULL,
    [email] varchar(100)  NULL
);
GO

-- Creating table 'GUEST_BOOKING'
CREATE TABLE [dbo].[GUEST_BOOKING] (
    [id] int IDENTITY(1,1) NOT NULL,
    [reservation_id] int  NOT NULL,
    [guest_id] varchar(12)  NOT NULL,
    [room_booked_id] int  NOT NULL
);
GO

-- Creating table 'INVOICEs'
CREATE TABLE [dbo].[INVOICEs] (
    [id] int IDENTITY(1,1) NOT NULL,
    [reservation_id] int  NOT NULL,
    [total_money] decimal(19,4)  NULL,
    [surcharge] float  NULL,
    [over_capacity_fee] float  NULL,
    [early_checkin_fee] float  NULL,
    [late_checkout_fee] float  NULL
);
GO

-- Creating table 'RESERVATIONs'
CREATE TABLE [dbo].[RESERVATIONs] (
    [id] int IDENTITY(1,1) NOT NULL,
    [date_created] datetime  NULL,
    [arrival] datetime  NULL,
    [departure] datetime  NULL,
    [status] varchar(20)  NULL,
    [main_guest] varchar(12)  NOT NULL,
    [early_checkin] bit  NULL,
    [late_checkout] bit  NULL
);
GO

-- Creating table 'ROOMs'
CREATE TABLE [dbo].[ROOMs] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] varchar(50)  NULL,
    [roomtype_id] int  NOT NULL,
    [notes] nvarchar(100)  NULL,
    [dirty] bit  NULL,
    [out_of_service] bit  NULL,
    [isActive] bit  NULL
);
GO

-- Creating table 'ROOM_BOOKED'
CREATE TABLE [dbo].[ROOM_BOOKED] (
    [id] int IDENTITY(1,1) NOT NULL,
    [reservation_id] int  NOT NULL,
    [room_id] int  NOT NULL
);
GO

-- Creating table 'ROOMTYPEs'
CREATE TABLE [dbo].[ROOMTYPEs] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(100)  NULL,
    [price] decimal(19,4)  NULL,
    [max_guest] int  NULL,
    [date_created] datetime  NOT NULL,
    [date_updated] datetime  NULL
);
GO

-- Creating table 'SERVICEs'
CREATE TABLE [dbo].[SERVICEs] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(100)  NULL,
    [price] decimal(19,4)  NULL,
    [isActive] bit  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'ACCOUNTs'
ALTER TABLE [dbo].[ACCOUNTs]
ADD CONSTRAINT [PK_ACCOUNTs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'CHARGES'
ALTER TABLE [dbo].[CHARGES]
ADD CONSTRAINT [PK_CHARGES]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'FOLIOs'
ALTER TABLE [dbo].[FOLIOs]
ADD CONSTRAINT [PK_FOLIOs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'GUESTs'
ALTER TABLE [dbo].[GUESTs]
ADD CONSTRAINT [PK_GUESTs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'GUEST_BOOKING'
ALTER TABLE [dbo].[GUEST_BOOKING]
ADD CONSTRAINT [PK_GUEST_BOOKING]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'INVOICEs'
ALTER TABLE [dbo].[INVOICEs]
ADD CONSTRAINT [PK_INVOICEs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'RESERVATIONs'
ALTER TABLE [dbo].[RESERVATIONs]
ADD CONSTRAINT [PK_RESERVATIONs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'ROOMs'
ALTER TABLE [dbo].[ROOMs]
ADD CONSTRAINT [PK_ROOMs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'ROOM_BOOKED'
ALTER TABLE [dbo].[ROOM_BOOKED]
ADD CONSTRAINT [PK_ROOM_BOOKED]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'ROOMTYPEs'
ALTER TABLE [dbo].[ROOMTYPEs]
ADD CONSTRAINT [PK_ROOMTYPEs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'SERVICEs'
ALTER TABLE [dbo].[SERVICEs]
ADD CONSTRAINT [PK_SERVICEs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [room_booked_id] in table 'FOLIOs'
ALTER TABLE [dbo].[FOLIOs]
ADD CONSTRAINT [FK__FOLIO__room_book__3B75D760]
    FOREIGN KEY ([room_booked_id])
    REFERENCES [dbo].[ROOM_BOOKED]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__FOLIO__room_book__3B75D760'
CREATE INDEX [IX_FK__FOLIO__room_book__3B75D760]
ON [dbo].[FOLIOs]
    ([room_booked_id]);
GO

-- Creating foreign key on [service_id] in table 'FOLIOs'
ALTER TABLE [dbo].[FOLIOs]
ADD CONSTRAINT [FK__FOLIO__service_i__3C69FB99]
    FOREIGN KEY ([service_id])
    REFERENCES [dbo].[SERVICEs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__FOLIO__service_i__3C69FB99'
CREATE INDEX [IX_FK__FOLIO__service_i__3C69FB99]
ON [dbo].[FOLIOs]
    ([service_id]);
GO

-- Creating foreign key on [guest_id] in table 'GUEST_BOOKING'
ALTER TABLE [dbo].[GUEST_BOOKING]
ADD CONSTRAINT [FK__GUEST_BOO__guest__35BCFE0A]
    FOREIGN KEY ([guest_id])
    REFERENCES [dbo].[GUESTs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__GUEST_BOO__guest__35BCFE0A'
CREATE INDEX [IX_FK__GUEST_BOO__guest__35BCFE0A]
ON [dbo].[GUEST_BOOKING]
    ([guest_id]);
GO

-- Creating foreign key on [main_guest] in table 'RESERVATIONs'
ALTER TABLE [dbo].[RESERVATIONs]
ADD CONSTRAINT [FK__RESERVATI__main___2E1BDC42]
    FOREIGN KEY ([main_guest])
    REFERENCES [dbo].[GUESTs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__RESERVATI__main___2E1BDC42'
CREATE INDEX [IX_FK__RESERVATI__main___2E1BDC42]
ON [dbo].[RESERVATIONs]
    ([main_guest]);
GO

-- Creating foreign key on [reservation_id] in table 'GUEST_BOOKING'
ALTER TABLE [dbo].[GUEST_BOOKING]
ADD CONSTRAINT [FK__GUEST_BOO__reser__34C8D9D1]
    FOREIGN KEY ([reservation_id])
    REFERENCES [dbo].[RESERVATIONs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__GUEST_BOO__reser__34C8D9D1'
CREATE INDEX [IX_FK__GUEST_BOO__reser__34C8D9D1]
ON [dbo].[GUEST_BOOKING]
    ([reservation_id]);
GO

-- Creating foreign key on [room_booked_id] in table 'GUEST_BOOKING'
ALTER TABLE [dbo].[GUEST_BOOKING]
ADD CONSTRAINT [FK__GUEST_BOO__room___36B12243]
    FOREIGN KEY ([room_booked_id])
    REFERENCES [dbo].[ROOM_BOOKED]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__GUEST_BOO__room___36B12243'
CREATE INDEX [IX_FK__GUEST_BOO__room___36B12243]
ON [dbo].[GUEST_BOOKING]
    ([room_booked_id]);
GO

-- Creating foreign key on [reservation_id] in table 'INVOICEs'
ALTER TABLE [dbo].[INVOICEs]
ADD CONSTRAINT [FK__INVOICE__reserva__3F466844]
    FOREIGN KEY ([reservation_id])
    REFERENCES [dbo].[RESERVATIONs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__INVOICE__reserva__3F466844'
CREATE INDEX [IX_FK__INVOICE__reserva__3F466844]
ON [dbo].[INVOICEs]
    ([reservation_id]);
GO

-- Creating foreign key on [reservation_id] in table 'ROOM_BOOKED'
ALTER TABLE [dbo].[ROOM_BOOKED]
ADD CONSTRAINT [FK__ROOM_BOOK__reser__30F848ED]
    FOREIGN KEY ([reservation_id])
    REFERENCES [dbo].[RESERVATIONs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__ROOM_BOOK__reser__30F848ED'
CREATE INDEX [IX_FK__ROOM_BOOK__reser__30F848ED]
ON [dbo].[ROOM_BOOKED]
    ([reservation_id]);
GO

-- Creating foreign key on [roomtype_id] in table 'ROOMs'
ALTER TABLE [dbo].[ROOMs]
ADD CONSTRAINT [FK__ROOM__roomtype_i__2B3F6F97]
    FOREIGN KEY ([roomtype_id])
    REFERENCES [dbo].[ROOMTYPEs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__ROOM__roomtype_i__2B3F6F97'
CREATE INDEX [IX_FK__ROOM__roomtype_i__2B3F6F97]
ON [dbo].[ROOMs]
    ([roomtype_id]);
GO

-- Creating foreign key on [room_id] in table 'ROOM_BOOKED'
ALTER TABLE [dbo].[ROOM_BOOKED]
ADD CONSTRAINT [FK__ROOM_BOOK__room___31EC6D26]
    FOREIGN KEY ([room_id])
    REFERENCES [dbo].[ROOMs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__ROOM_BOOK__room___31EC6D26'
CREATE INDEX [IX_FK__ROOM_BOOK__room___31EC6D26]
ON [dbo].[ROOM_BOOKED]
    ([room_id]);
GO

-- --------------------------------------------------
-- Initialize some stuff
-- --------------------------------------------------

-- Creating First Admin, Password = 1
INSERT INTO [dbo].[ACCOUNTs]
           ([username]
           ,[password]
           ,[permission])
     VALUES
           ('admin','cdd96d3cc73d1dbdaffa03cc6cd7339b','Admin')
GO


-- Creating First Charges
INSERT INTO [dbo].[CHARGES]
           ([id]
           ,[surcharge]
           ,[over_capacity_fee]
           ,[early_checkin_fee]
           ,[late_checkout_fee])
     VALUES
           (1,0,0,0,0)
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------