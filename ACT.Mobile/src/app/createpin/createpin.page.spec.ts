import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { CreatepinPage } from './createpin.page';

describe('CreatepinPage', () => {
  let component: CreatepinPage;
  let fixture: ComponentFixture<CreatepinPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatepinPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(CreatepinPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
