
IF NOT EXISTS (SELECT 1 FROM frmk.SettingDefinition where Settingname='tmg.equinox.net.PdfConformance')
BEGIN
	insert frmk.SettingDefinition
	select 'DocCompliance',	'tmg.equinox.net.PdfConformance',	'PdfA1a,PdfA1b',	'PDF Compliance Mode',	'Applying Compliance on Pdf- Option avilable PdfA1b,PdfA1a,PdfA2b,PdfA2u,PdfA2a',	0,	0,	NULL
END
IF NOT EXISTS (SELECT 1 FROM frmk.SettingDefinition where Settingname='tmg.equinox.net.PdfConformance.OnOff')
BEGIN
	insert frmk.SettingDefinition
	select 'DocCompliance',	'tmg.equinox.net.PdfConformance.OnOff',	'ON',	'PDF Compliance Mode',	'Applying Compliance on Pdf- Option avilable PdfA1b,PdfA1a,PdfA2b,PdfA2u,PdfA2a',	0,	0,	NULL
END
IF NOT EXISTS (SELECT 1 FROM frmk.SettingDefinition where Settingname='tmg.equinox.net.PdfConformance.Author')
BEGIN
	insert frmk.SettingDefinition
	select 'DocCompliance',	'tmg.equinox.net.PdfConformance.Author',	'Centers for Medicare & Medicaid Services',	'PDF Compliance',	'Information Used in PDF properties-Author',	0,	0,	NULL
END
Print 'Successfully Executed'


