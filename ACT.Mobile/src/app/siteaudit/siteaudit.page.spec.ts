import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { SiteauditPage } from './siteaudit.page';

describe('SiteauditPage', () => {
  let component: SiteauditPage;
  let fixture: ComponentFixture<SiteauditPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SiteauditPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(SiteauditPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
