import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { ConfirmdeletesiteauditComponent } from './confirmdeletesiteaudit.component';

describe('ConfirmdeletesiteauditComponent', () => {
  let component: ConfirmdeletesiteauditComponent;
  let fixture: ComponentFixture<ConfirmdeletesiteauditComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfirmdeletesiteauditComponent ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(ConfirmdeletesiteauditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
