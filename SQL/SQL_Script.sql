create database PI_Grade_Calculator COLLATE Croatian_CI_AS
go

use PI_Grade_Calculator
go

create table Uloga (
    IDUloga int primary key identity (1, 1),
    Naziv nvarchar(50) unique not null,
);
go

create table Korisnik (
    IDKorisnik int primary key identity (1, 1),
    KorisnickoIme nvarchar(50) unique not null,
    EPosta varchar(50) unique not null,
    LozinkaHash nvarchar(255) not null,
	LozinkaSalt nvarchar(255) not null,
	UkupnaOcjena float default 0 not null,
	UlogaID int foreign key references Uloga(IDUloga) not null
);
go

create table Godina (
    IDGodina int primary key identity (1, 1),
    Naziv nvarchar(50) not null,
	Prosjek float default 0 not null,
	KorisnikID int foreign key (KorisnikID) references Korisnik(IDKorisnik)
);
go

create table Predmet (
    IDPredmet int primary key identity (1, 1),
    Naziv nvarchar(50) not null,
	Prosjek float,
	GodinaID int foreign key (GodinaID) references Godina(IDGodina)
);
go

create table Ocjena (
    IDOcjena int primary key identity (1, 1),
    Vrijednost int,
	PredmetID int foreign key (PredmetID) references Predmet(IDPredmet)
);
go

create table [Log] (
    IDLog int primary key identity (1, 1),
    Opis    nvarchar(100) not null,
    Vrijeme datetime not null
);
go

insert into Uloga (Naziv)
values 
('korisnik'),
('admin');
go

insert into Korisnik (KorisnickoIme, EPosta, LozinkaHash, LozinkaSalt, UlogaID)
values 
('pero', 'pp@gmail.com', 'RvG97AOUBIEa5uIry4X2C2mNpPRjnnKlzeMJVLwdxYY=', 'yzh77hIWxfB1QD5xaGRILA==', 1),
('viki', 'vpetrina@algebra.hr', '/uZ8jT1CFtFYZl4ekPrpZD8aDH8Y10fCjvFmj4dNAqA=', 'BvsheJUW9SAT3qThbECW/w==', 1),
('mico', 'mvukusic@algebra.hr', 'QyGAo7ThZ0qraoxKWlkEwNdTbMOI+Edg1O4QAU3P5V0=', 'JIVCnXlad/nfiluhB0Bt5g==', 1),
('miki', 'mjurela@algebra.hr', 'z9jPxckOlhXLDrswPa6LGX+ZcWRMUQNym+JNAJBHRTw=', '01yQE0ZAT/vooUx3ohzOvA==', 1),
('sudo', 'superUser@do.com', 'Z0a6GCPtblAibhYVdDtIXxQadWf0Jg8/ygKUEix+Esg=', '/zL7svjOhTgRAeJxMIgcNQ==', 2);
go
