import { StreamCategoryKey } from "../enums/stream-categories";

export interface LiveStream {
    id: string;
    streamName: string;
    streamDescription: string;
    streamCategory: StreamCategoryKey;
    userId: string;
    thumbnailUrl?: string;
    streamUrl?: string;
    streamKey?: string;
    views: number;
    username?: string;
}