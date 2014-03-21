--Insert default data for RFID Service Configuration database
--Sqlite database
/*
--old insert statements of database tables.
INSERT INTO ConfigItemGroup (ID, Name, Description) VALUES
(1, 'Read User Memory', NULL);
INSERT INTO ConfigItemGroup (ID, Name, Description) VALUES
(2, 'Auto Start Mode', NULL);
INSERT INTO ConfigItemGroup (ID, Name, Description) VALUES
(3, 'Auto Stop Mode', NULL);
INSERT INTO ConfigItemGroup (ID, Name, Description) VALUES
(4, 'Low Duty Cycle', NULL);

INSERT INTO ConfigItemType (ID, Name, Description, RegEx) VALUES
(1, 'Integer', NULL, '^\d+$');
INSERT INTO ConfigItemType (ID, Name, Description, RegEx) VALUES
(2, 'Boolean', NULL, '(true|false)');
INSERT INTO ConfigItemType (ID, Name, Description, RegEx) VALUES
(3, 'AutoStopMode', NULL, '(Duration|GpiTrigger)');
INSERT INTO ConfigItemType (ID, Name, Description, RegEx) VALUES
(4, 'String', NULL, NULL);

--defaults used by the application when user adds a new Reader
INSERT INTO Reader (ID, HostName, CurrentStatus, LastPing, ReaderID, IsDefault) VALUES
(1, 'Host Name', 'Current Status', DateTime('now'), 'Reader Id', 1);
INSERT INTO Antenna (ID, Port, TxPowerIndBm, FK_ID_Reader, IsDefault) VALUES
(1, 1, 30, 1, 1);
INSERT INTO Antenna (ID, Port, TxPowerIndBm, FK_ID_Reader, IsDefault) VALUES
(2, 2, 30, 1, 1);
--Read User Memory--
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('Enabled', 'true', 1, 2, 1);
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('Word Count', '2', 1, 1, 1);
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('Word Pointer', '0', 1, 1, 1);
--Auto Start Mode--
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('Enabled', 'true', 2, 2, 1);
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('GPI Level', 'true', 2, 2, 1);
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('Port', '1', 2, 1, 1);
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('Debounce', '500', 2, 1, 1);
--Auto Stop Mode--
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('Enabled', 'true', 3, 2, 1);
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('Mode', 'Duration', 3, 3, 1);
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('Debounce', '500', 3, 1, 1);
INSERT INTO ConfigItem (NAME, VALUE, FK_ID_CONFIG_ITEM_GROUP, FK_ID_CONFIG_ITEM_TYPE, FK_ID_Reader) VALUES
('Duration In MS', '10000', 3, 1, 1);
*/