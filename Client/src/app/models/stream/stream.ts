import { StreamCategory } from "../enums/stream-categories";


export interface LiveStream {
    id: string;
    streamName: string;
    streamDescription: string;
    streamCategory: StreamCategory;
    userId: string;
    thumbnailUrl?: string;
    streamUrl?: string;
    streamKey?: string;
    views: number;
    currentViewers?: number;
    username? : string;
}