import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { OutstandingreportPage } from './outstandingreport.page';

describe('OutstandingreportPage', () => {
  let component: OutstandingreportPage;
  let fixture: ComponentFixture<OutstandingreportPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OutstandingreportPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(OutstandingreportPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
