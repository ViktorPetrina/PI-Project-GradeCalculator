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
    EPosta varchar(50) not null,
    LozinkaHash nvarchar(255) not null,
	LozinkaSalt nvarchar(255) not null,
	UkupnaOcjena float default 0 not null,
	UlogaID int foreign key references Uloga(IDUloga) not null
);
go

create table Godina (
    IDGodina int primary key identity (1, 1),
    Naziv nvarchar(50) unique not null,
	Prosjek float,
	KorisnikID int foreign key (KorisnikID) references Korisnik(IDKorisnik)
);
go

create table Predmet (
    IDPredmet int primary key identity (1, 1),
    Naziv nvarchar(50) unique not null,
	Prosjek float,
	GodinaID int foreign key (GodinaID) references Godina(IDGodina)
);
go

create table Ocjena (
    IDOcjena int primary key identity (1, 1),
    Vrijednost int not null,
	PredmetID int foreign key (PredmetID) references Predmet(IDPredmet)
);
go

create table [Log] (
    IDOcjena int primary key identity (1, 1),
    Opis    nvarchar(100) not null,
    Vrijeme datetime not null
);
go

insert into Uloga (Naziv)
values 
('korisnik'),
('admin');
go
