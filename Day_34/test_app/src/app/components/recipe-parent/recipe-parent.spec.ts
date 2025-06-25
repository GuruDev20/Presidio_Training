import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RecipeParent } from './recipe-parent';
import { RecipeService } from '../../services/recipe';
import { of } from 'rxjs';
import { RecipeChild } from '../recipe-child/recipe-child';

describe('RecipeParentComponent', () => {
    let component: RecipeParent;
    let fixture: ComponentFixture<RecipeParent>;
    let mockService: jasmine.SpyObj<RecipeService>;

    beforeEach(async () => {
        const spy = jasmine.createSpyObj('RecipeService', ['getRecipes']);

        await TestBed.configureTestingModule({
            imports: [RecipeParent, RecipeChild],
            providers: [{ provide: RecipeService, useValue: spy }]
        }).compileComponents();

        fixture = TestBed.createComponent(RecipeParent);
        component = fixture.componentInstance;
        mockService = TestBed.inject(RecipeService) as jasmine.SpyObj<RecipeService>;
    });

    it('should load and display simplified recipes', () => {
        mockService.getRecipes.and.returnValue(of({
        recipes: [{
            id: 1,
            name: 'Basic Recipe',
            ingredients: ['Egg', 'Milk'],
            prepTimeMinutes: 5,
            cookTimeMinutes: 10,
            image: 'basic.jpg'
        }]
        }));

        fixture.detectChanges();
        expect(component.recipes.length).toBe(1);
    });
});
