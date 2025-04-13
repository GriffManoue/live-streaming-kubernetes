export enum StreamCategory {
    Gaming = "Gaming",
    Music = "Music",
    Sports = "Sports",
    Art = "Art",
    Cooking = "Cooking",
    Education = "Education",
    Travel = "Travel",
    TalkShows = "TalkShows",
    News = "News",
    Technology = "Technology",
    Science = "Science",
    HealthAndFitness = "HealthAndFitness",
    FashionAndBeauty = "FashionAndBeauty",
    FoodAndDrink = "FoodAndDrink",
    PetsAndAnimals = "PetsAndAnimals",
    DiyAndCrafts = "DiyAndCrafts"
}

export const StreamCategoryIcons: Record<StreamCategory, string> = {
    [StreamCategory.Gaming]: "pi pi-desktop",
    [StreamCategory.Music]: "pi pi-volume-up",
    [StreamCategory.Sports]: "pi pi-star",
    [StreamCategory.Art]: "pi pi-palette",
    [StreamCategory.Cooking]: "pi pi-shopping-bag",
    [StreamCategory.Education]: "pi pi-book",
    [StreamCategory.Travel]: "pi pi-globe",
    [StreamCategory.TalkShows]: "pi pi-comments",
    [StreamCategory.News]: "pi pi-wave-pulse",
    [StreamCategory.Technology]: "pi pi-desktop",
    [StreamCategory.Science]: "pi pi-cog",
    [StreamCategory.HealthAndFitness]: "pi pi-heart",
    [StreamCategory.FashionAndBeauty]: "pi pi-eye",
    [StreamCategory.FoodAndDrink]: "pi pi-shopping-cart",
    [StreamCategory.PetsAndAnimals]: "pi pi-prime",
    [StreamCategory.DiyAndCrafts]: "pi pi-wrench"
};
