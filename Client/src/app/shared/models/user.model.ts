export interface User {
  id: string;
  username: string;
  email: string;
  token?: string;
  profilePictureUrl?: string;
  bio?: string;
  followersCount?: number;
  followingCount?: number;
  isFollowing?: boolean;
  isStreaming?: boolean;
  createdAt?: Date;
}
