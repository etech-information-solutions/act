import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { OutstandingpalletsperclientPage } from './outstandingpalletsperclient.page';

describe('OutstandingpalletsperclientPage', () => {
  let component: OutstandingpalletsperclientPage;
  let fixture: ComponentFixture<OutstandingpalletsperclientPage>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ OutstandingpalletsperclientPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(OutstandingpalletsperclientPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
