export interface Stream {
  id: string;
  title: string;
  description?: string;
  thumbnailUrl?: string;
  streamUrl: string;
  isLive: boolean;
  viewerCount: number;
  userId: string;
  username: string;
  userProfilePictureUrl?: string;
  category?: string;
  tags?: string[];
  startedAt?: Date;
  endedAt?: Date;
  createdAt: Date;
}
