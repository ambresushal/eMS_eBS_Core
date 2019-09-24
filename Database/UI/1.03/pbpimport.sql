
alter table PBP.PBPMatchConfig add IsEGWPPlan bit default 0
go
update PBP.PBPMatchConfig set IsEGWPPlan=0 


