export class LiveStream {
    id: string;
    streamId: string;
    thumbnailUrl: string;
    views: number;
    isLive: boolean;

    constructor(id: string, streamId: string, thumbnailUrl: string, views: number, isLive: boolean) {
        this.id = id;
        this.streamId = streamId;
        this.thumbnailUrl = thumbnailUrl;
        this.views = views;
        this.isLive = isLive;
    }
}