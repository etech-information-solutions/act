import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { ChataddPage } from './chatadd.page';

describe('ChataddPage', () => {
  let component: ChataddPage;
  let fixture: ComponentFixture<ChataddPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChataddPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(ChataddPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
