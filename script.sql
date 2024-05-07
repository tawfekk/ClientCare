create table AspNetRoles
(
    Id               nvarchar(450) not null
        constraint PK_AspNetRoles
            primary key,
    ConcurrencyStamp nvarchar(max),
    Name             nvarchar(256),
    NormalizedName   nvarchar(256)
);

create table AspNetRoleClaims
(
    Id         int identity
        constraint PK_AspNetRoleClaims
            primary key,
    ClaimType  nvarchar(max),
    ClaimValue nvarchar(max),
    RoleId     nvarchar(450) not null
        constraint FK_AspNetRoleClaims_AspNetRoles_RoleId
            references AspNetRoles
            on delete cascade
);

create index IX_AspNetRoleClaims_RoleId
    on AspNetRoleClaims (RoleId);

create unique index RoleNameIndex
    on AspNetRoles (NormalizedName)
    where [NormalizedName] IS NOT NULL;

create table AspNetUsers
(
    Id                   nvarchar(450) not null
        constraint PK_AspNetUsers
            primary key,
    AccessFailedCount    int           not null,
    ConcurrencyStamp     nvarchar(max),
    Email                nvarchar(256),
    EmailConfirmed       bit           not null,
    LockoutEnabled       bit           not null,
    LockoutEnd           datetimeoffset,
    NormalizedEmail      nvarchar(256),
    NormalizedUserName   nvarchar(256),
    PasswordHash         nvarchar(max),
    PhoneNumber          nvarchar(max),
    PhoneNumberConfirmed bit           not null,
    SecurityStamp        nvarchar(max),
    TwoFactorEnabled     bit           not null,
    UserName             nvarchar(256),
    FirstName            nvarchar(max),
    LastName             nvarchar(max),
    Picture              nvarchar(max)
);

create table AspNetUserClaims
(
    Id         int identity
        constraint PK_AspNetUserClaims
            primary key,
    ClaimType  nvarchar(max),
    ClaimValue nvarchar(max),
    UserId     nvarchar(450) not null
        constraint FK_AspNetUserClaims_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade
);

create index IX_AspNetUserClaims_UserId
    on AspNetUserClaims (UserId);

create table AspNetUserLogins
(
    LoginProvider       nvarchar(450) not null,
    ProviderKey         nvarchar(450) not null,
    ProviderDisplayName nvarchar(max),
    UserId              nvarchar(450) not null
        constraint FK_AspNetUserLogins_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    constraint PK_AspNetUserLogins
        primary key (LoginProvider, ProviderKey)
);

create index IX_AspNetUserLogins_UserId
    on AspNetUserLogins (UserId);

create table AspNetUserRoles
(
    UserId nvarchar(450) not null
        constraint FK_AspNetUserRoles_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    RoleId nvarchar(450) not null
        constraint FK_AspNetUserRoles_AspNetRoles_RoleId
            references AspNetRoles
            on delete cascade,
    constraint PK_AspNetUserRoles
        primary key (UserId, RoleId)
);

create index IX_AspNetUserRoles_RoleId
    on AspNetUserRoles (RoleId);

create table AspNetUserTokens
(
    UserId        nvarchar(450) not null
        constraint FK_AspNetUserTokens_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    LoginProvider nvarchar(450) not null,
    Name          nvarchar(450) not null,
    Value         nvarchar(max),
    constraint PK_AspNetUserTokens
        primary key (UserId, LoginProvider, Name)
);

create index EmailIndex
    on AspNetUsers (NormalizedEmail);

create unique index UserNameIndex
    on AspNetUsers (NormalizedUserName)
    where [NormalizedUserName] IS NOT NULL;

create table Brancher
(
    Id              int identity
        primary key,
    Name            nvarchar(max),
    Branchedirektør nvarchar(max)
);

create table Netværk
(
    Id       int identity
        primary key,
    Name     nvarchar(max) not null,
    MedlemId int           not null
);

create table RelationsAnsvarlige
(
    Id   int identity
        primary key,
    Name nvarchar(max)
);

create table Medlemmer
(
    Id                   int identity
        primary key,
    Email                nvarchar(max) not null,
    Name                 nvarchar(max) not null,
    Direktør             nvarchar(max),
    AntalAnsatte         nvarchar(max),
    CVR                  nvarchar(max),
    RelationsansvarligId int
        constraint Medlemmer_RelationsAnsvarlige_Id_fk
            references RelationsAnsvarlige,
    BrancheId            int           not null
        constraint Medlemmer_Brancher_Id_fk
            references Brancher,
    Kontigent            decimal,
    Indmeldelsesdato     datetime      not null
);

create table MedlemNetværk
(
    MedlemId  int not null
        constraint MedlemNetværk_Medlemmer_Id_fk
            references Medlemmer,
    NetværkId int not null
        constraint MedlemNetværk_Netværk_Id_fk
            references Netværk,
    constraint MedlemNetværk_pk
        primary key (MedlemId, NetværkId)
);

create table __EFMigrationsHistory
(
    MigrationId    nvarchar(150) not null
        constraint PK___EFMigrationsHistory
            primary key,
    ProductVersion nvarchar(32)  not null
);

