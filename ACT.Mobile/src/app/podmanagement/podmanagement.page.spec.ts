import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { PodmanagementPage } from './podmanagement.page';

describe('PodmanagementPage', () => {
  let component: PodmanagementPage;
  let fixture: ComponentFixture<PodmanagementPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PodmanagementPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(PodmanagementPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});