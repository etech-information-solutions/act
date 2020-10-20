import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { OutstandingpalletspersitePage } from './outstandingpalletspersite.page';

describe('OutstandingpalletspersitePage', () => {
  let component: OutstandingpalletspersitePage;
  let fixture: ComponentFixture<OutstandingpalletspersitePage>;

  beforeEach(async(() => {
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
