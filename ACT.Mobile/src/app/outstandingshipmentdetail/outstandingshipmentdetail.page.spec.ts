import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { OutstandingshipmentdetailPage } from './outstandingshipmentdetail.page';

describe('OutstandingshipmentdetailPage', () => {
  let component: OutstandingshipmentdetailPage;
  let fixture: ComponentFixture<OutstandingshipmentdetailPage>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ OutstandingshipmentdetailPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(OutstandingshipmentdetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
