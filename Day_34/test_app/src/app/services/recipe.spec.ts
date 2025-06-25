import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { RecipeService } from "./recipe"
import { TestBed } from "@angular/core/testing";

describe("ReceipeService",()=>{
    let service:RecipeService;
    let httpMock:HttpTestingController;

    beforeEach(()=>{
        TestBed.configureTestingModule({
            imports:[HttpClientTestingModule],
            providers:[RecipeService]
        });

        service=TestBed.inject(RecipeService);
        httpMock=TestBed.inject(HttpTestingController);
    });

    it('should fetch recipes',()=>{
        const dummyResponse={
            recipes:[{
                id:1,
                name:'Test Recipe',
                ingredients:['A','B'],
                prepTimeMinutes:5,
                cookTimeMinutes:10,
                image:'test.jpg'
            }]
        };

        service.getRecipes().subscribe(res=>{
            expect(res.recipes.length).toBe(1);
            expect(res.recipes[0].name).toBe('Test Recipe');
        });

        const req=httpMock.expectOne('https://dummyjson.com/recipes');
        expect(req.request.method).toBe('GET');
        req.flush(dummyResponse);
    });

    afterEach(()=>{
        httpMock.verify();
    })
})