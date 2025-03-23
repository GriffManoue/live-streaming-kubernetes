import { StreamCategoryKey } from "../enums/stream-categories";

export class LiveStream {
    id: string;
    streamName: string;
    streamDescription: string;
    streamCategory: StreamCategoryKey;
    userId: string;
    isActive: boolean = true;

    constructor(id: string, streamName: string, streamDescription: string, streamCategory: StreamCategoryKey, userId: string) {
        this.id = id;
        this.streamName = streamName;
        this.streamDescription = streamDescription;
        this.streamCategory = streamCategory;
        this.userId = userId;
    }
}