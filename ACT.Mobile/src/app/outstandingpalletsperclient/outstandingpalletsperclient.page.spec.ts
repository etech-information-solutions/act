import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { OutstandingpalletsperclientPage } from './outstandingpalletsperclient.page';

describe('OutstandingpalletsperclientPage', () => {
  let component: OutstandingpalletsperclientPage;
  let fixture: ComponentFixture<OutstandingpalletsperclientPage>;

  beforeEach(async(() => {
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
