import { NgModule } from "@angular/core";
import { FruitFreshnessCard } from "./fruit-freshness-card/fruit-freshness-card.component";
import { HttpClientModule } from "@angular/common/http";
import { CommonModule } from "@angular/common";

@NgModule({
    declarations: [
        FruitFreshnessCard,
    ],
    imports: [
        HttpClientModule , 
        CommonModule
    ],
    exports: [
        FruitFreshnessCard
    ],
})
export class SharedModule {}