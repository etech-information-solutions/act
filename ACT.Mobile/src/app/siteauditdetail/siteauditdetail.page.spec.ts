import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { SiteauditdetailPage } from './siteauditdetail.page';

describe('SiteauditdetailPage', () => {
  let component: SiteauditdetailPage;
  let fixture: ComponentFixture<SiteauditdetailPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SiteauditdetailPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(SiteauditdetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
