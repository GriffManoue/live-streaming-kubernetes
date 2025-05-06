CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "Username" text NOT NULL,
    "Password" text NOT NULL,
    "Email" text NOT NULL,
    "FirstName" text,
    "LastName" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "IsLive" boolean NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);


CREATE TABLE "LiveStreams" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "StreamName" text NOT NULL,
    "StreamDescription" text NOT NULL,
    "StreamCategory" integer NOT NULL,
    "ThumbnailUrl" text,
    "StreamUrl" text,
    "StreamKey" text,
    "Views" integer NOT NULL,
    CONSTRAINT "PK_LiveStreams" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_LiveStreams_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);


CREATE TABLE "UserFollowers" (
    "FollowersId" uuid NOT NULL,
    "FollowingId" uuid NOT NULL,
    CONSTRAINT "PK_UserFollowers" PRIMARY KEY ("FollowersId", "FollowingId"),
    CONSTRAINT "FK_UserFollowers_Users_FollowersId" FOREIGN KEY ("FollowersId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserFollowers_Users_FollowingId" FOREIGN KEY ("FollowingId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);


CREATE UNIQUE INDEX "IX_LiveStreams_UserId" ON "LiveStreams" ("UserId");


CREATE INDEX "IX_UserFollowers_FollowingId" ON "UserFollowers" ("FollowingId");


/*
Table Users {
  Id uuid [primary key]
  Username text
  Password text
  Email text
  FirstName text
  LastName text
  CreatedAt timestamp [not null]
  IsLive boolean [not null]
}

Table LiveStreams {
  Id uuid [primary key]
  UserId uuid [not null]
  StreamName text
  StreamDescription text
  StreamCategory integer
  ThumbnailUrl text
  StreamUrl text
  StreamKey text
  Views integer
}

Table UserFollowers {
  FollowersId uuid [not null]
  FollowingId uuid [not null]
  // Composite primary key
  primary key (FollowersId, FollowingId)
}

Ref: LiveStreams.UserId > Users.Id
Ref: UserFollowers.FollowersId > Users.Id
Ref: UserFollowers.FollowingId > Users.Id
*/