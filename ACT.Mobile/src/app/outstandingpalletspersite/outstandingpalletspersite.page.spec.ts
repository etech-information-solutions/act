import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { OutstandingpalletspersitePage } from './outstandingpalletspersite.page';

describe('OutstandingpalletspersitePage', () => {
  let component: OutstandingpalletspersitePage;
  let fixture: ComponentFixture<OutstandingpalletspersitePage>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ OutstandingpalletspersitePage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(OutstandingpalletspersitePage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
