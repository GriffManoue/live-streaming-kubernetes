export const StreamCategories = {
    Gaming: { name: "Gaming", icon: "pi pi-desktop" },
    Music: { name: "Music", icon: "pi pi-volume-up" },
    Sports: { name: "Sports", icon: "pi pi-star" },
    Art: { name: "Art", icon: "pi pi-palette" },
    Cooking: { name: "Cooking", icon: "pi pi-shopping-bag" },
    Education: { name: "Education", icon: "pi pi-book" },
    Travel: { name: "Travel", icon: "pi pi-globe" },
    TalkShows: { name: "TalkShows", icon: "pi pi-comments" },
    News: { name: "News", icon: "pi pi-wave-pulse" },
    Technology: { name: "Technology", icon: "pi pi-desktop" },
    Science: { name: "Science", icon: "pi pi-cog" },
    HealthAndFitness: { name: "HealthAndFitness", icon: "pi pi-heart" },
    FashionAndBeauty: { name: "FashionAndBeauty", icon: "pi pi-eye" },
    FoodAndDrink: { name: "FoodAndDrink", icon: "pi pi-shopping-cart" },
    PetsAndAnimals: { name: "PetsAndAnimals", icon: "pi pi-prime" },
    DiyAndCrafts: { name: "DiyAndCrafts", icon: "pi pi-wrench" }
} as const;

// Type for StreamCategory keys
export type StreamCategoryKey = keyof typeof StreamCategories;

// Interface for category structure
export interface StreamCategory {
    name: string;
    icon: string;
}
