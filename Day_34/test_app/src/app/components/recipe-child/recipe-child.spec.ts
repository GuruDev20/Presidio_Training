import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RecipeChild } from './recipe-child';

describe('RecipeChildComponent', () => {
    let component: RecipeChild;
    let fixture: ComponentFixture<RecipeChild>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports:[RecipeChild]
        }).compileComponents();

        fixture = TestBed.createComponent(RecipeChild);
        component = fixture.componentInstance;
    });

    it('should render simplified recipe info', () => {
        component.recipe = {
            id: 1,
            name: 'Test Dish',
            ingredients: ['Onion', 'Garlic'],
            prepTimeMinutes: 10,
            cookTimeMinutes: 15,
            image: 'test.jpg'
        };
        fixture.detectChanges();

        const compiled = fixture.nativeElement;
        expect(compiled.querySelector('h3').textContent).toContain('Test Dish');
        expect(compiled.querySelectorAll('li').length).toBe(2);
    });
});
