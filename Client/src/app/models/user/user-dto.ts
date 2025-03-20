export class UserDto {
  id: string;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  phone?: string;
  createdAt: Date;
  isActive: boolean;
  followersCount: number;
  followingCount: number;

  constructor(
    id: string,
    username: string,
    email: string,
    createdAt: Date,
    isActive: boolean,
    followersCount: number,
    followingCount: number,
    firstName?: string,
    lastName?: string,
    phone?: string,
  ) {
    this.id = id;
    this.username = username;
    this.email = email;
    this.firstName = firstName;
    this.lastName = lastName;
    this.phone = phone;
    this.createdAt = createdAt;
    this.isActive = isActive;
    this.followersCount = followersCount;
    this.followingCount = followingCount;
  }
}
