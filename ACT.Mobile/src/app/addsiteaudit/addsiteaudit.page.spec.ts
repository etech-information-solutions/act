import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { AddsiteauditPage } from './addsiteaudit.page';

describe('AddsiteauditPage', () => {
  let component: AddsiteauditPage;
  let fixture: ComponentFixture<AddsiteauditPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddsiteauditPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(AddsiteauditPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
