import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { PopoverController } from '@ionic/angular';

@Component({
  selector: 'app-exitappconfirm',
  templateUrl: './exitappconfirm.component.html',
  styleUrls: ['./exitappconfirm.component.scss'],
})
export class ExitappconfirmComponent implements OnInit
{

  constructor( public auth: UserService, public popCtrl: PopoverController )
  {

  }

  ngOnInit()
  {

  }

  Yes()
  {
    this.auth.ExitApp = true;
    this.popCtrl.dismiss();
  }

  No()
  {
    this.auth.ExitApp = false;
    this.popCtrl.dismiss();
  }
}
