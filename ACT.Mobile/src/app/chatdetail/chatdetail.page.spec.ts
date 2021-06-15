import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { ChatdetailPage } from './chatdetail.page';

describe('ChatdetailPage', () => {
  let component: ChatdetailPage;
  let fixture: ComponentFixture<ChatdetailPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatdetailPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(ChatdetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
